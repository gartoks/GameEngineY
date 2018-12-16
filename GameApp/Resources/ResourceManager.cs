using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using GameApp.Application;
using GameApp.Modding;
using GameEngine.Logging;
using GameEngine.Resources;
using GameEngine.Resources.Loaders;

namespace GameApp.Resources {
    internal class ResourceManager : IResourceManager {
        private static ResourceManager instance;
        internal static ResourceManager Instance {
            get => ResourceManager.instance;
            private set { if (ResourceManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private readonly Dictionary<Type, Dictionary<Type, IResourceLoader>> loaders;

        private readonly Dictionary<string, (Type type, IResource res)> globalResources;
        private readonly Dictionary<string, (Type type, IResource res)> sceneResources;

        private readonly SortedDictionary<int, Queue<ResourceLoadingTask>> loadingQueue;
        private int loadingQueueSize;
        private int highestPriority;

        private readonly ConcurrentDictionary<string, object> queuedResources;

        internal ResourceManager() {
            Instance = this;

            this.loaders = new Dictionary<Type, Dictionary<Type, IResourceLoader>>();

            this.globalResources = new Dictionary<string, (Type, IResource)>();
            this.sceneResources = new Dictionary<string, (Type, IResource)>();

            this.loadingQueue = new SortedDictionary<int, Queue<ResourceLoadingTask>>();
            this.loadingQueueSize = 0;
            this.highestPriority = -1;

            this.queuedResources = new ConcurrentDictionary<string, object>();
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            RegisterResourceLoader(new TextLoader());
            RegisterResourceLoader(new TextureLoader());
            RegisterResourceLoader(new ShaderLoader());
            RegisterResourceLoader(new TextureAtlasLoader());
            RegisterResourceLoader(new FontLoader());
        }

        internal void ContinueLoading() {
            if (!IsLoading)
                return;

            while (this.highestPriority >= 0 && (!this.loadingQueue.ContainsKey(this.highestPriority) || this.loadingQueue[this.highestPriority].Count == 0))
                this.highestPriority--;

            if (this.highestPriority < 0)
                return;

            ResourceLoadingTask resourceLoadingTask;
            lock (this.loadingQueue) {
                resourceLoadingTask = this.loadingQueue[this.highestPriority].Dequeue();
                loadingQueueSize--;
            }

            IResourceLoader loader = GetLoader(resourceLoadingTask.ResourceType, resourceLoadingTask.ResourceLoadingParametersType);

            if (loader == null) {
                Logging.Log.Instance.WriteLine($"Resource loader for resource of type '{resourceLoadingTask.ResourceType}' and loading parameters of type '{resourceLoadingTask.ResourceLoadingParametersType}' does not exist.", LogType.Error);
                return;
            }

            IResourceLoadingParameters resourceLoadingParameters = resourceLoadingTask.ResourceLoadingParameters;
            IResource resource = loader.LoadResource(resourceLoadingTask.ResourceIdentifier, resourceLoadingParameters);

            if (resourceLoadingTask.GlobalResource)
                this.globalResources[resourceLoadingTask.ResourceIdentifier] = (resourceLoadingTask.ResourceType, resource);
            else
                this.sceneResources[resourceLoadingTask.ResourceIdentifier] = (resourceLoadingTask.ResourceType, resource);

            this.queuedResources.TryRemove(resourceLoadingTask.ResourceIdentifier, out object ignored);
        }

        public bool TryGetResource<R>(string resourceIdentifer, out R resource, bool waitForLoading = true) {
            resource = default(R);

            (Type type, IResource res) resourceData;
            if (this.sceneResources.TryGetValue(resourceIdentifer, out resourceData)) {
            } else if (this.globalResources.TryGetValue(resourceIdentifer, out resourceData)) {
            } else if (waitForLoading && this.queuedResources.ContainsKey(resourceIdentifer)) {
                while (this.queuedResources.ContainsKey(resourceIdentifer))
                    Thread.Sleep(100);
                return TryGetResource<R>(resourceIdentifer, out resource, false);
            } else
                return false;

            if (resourceData.type != typeof(R))
                return false;

            Resource<R> res = resourceData.res as Resource<R>;
            resource = res.Data;

            return true;
        }

        public void LoadResource<R, RLP>(string modID, string resourceIdentifier, RLP resourceLoadingParameters, int loadingPriority, bool globalResource = false) where RLP : ResourceLoadingParameters<R> {
            if (string.IsNullOrWhiteSpace(modID) || ModManager.Instance.IsModLoaded(modID)) {
                Log.WriteLine($"Could not load resource '{resourceIdentifier}' from mod '{modID}'. Mod does not exist.", LogType.Error);
                return;
            }

            if (this.queuedResources.ContainsKey(resourceIdentifier)) {
                Log.WriteLine($"Resource with identifier '{resourceIdentifier}' is already queued for loading.", LogType.Error);
                return;
            }

            FieldInfo pathsInfo = null;
            Type baseLoadingParametersType = resourceLoadingParameters.GetType().BaseType;
            while ((pathsInfo = baseLoadingParametersType.GetField("FilePaths", BindingFlags.Instance | BindingFlags.Public)) == null)
                baseLoadingParametersType = baseLoadingParametersType.BaseType;

            IEnumerable<string> rawPaths = resourceLoadingParameters.FilePaths;
            string[] newPaths = new string[rawPaths.Count()];
            int i = 0;
            foreach (string rawPath in rawPaths) {
                newPaths[i] = Path.Combine(AppConstants.Directories.MODS, modID, rawPath);
                i++;
            }

            pathsInfo.SetValue(resourceLoadingParameters, newPaths);

            ResourceLoadingTask loadingTask = new ResourceLoadingTask(resourceIdentifier, typeof(RLP), resourceLoadingParameters, typeof(R), globalResource);
            if (!this.loadingQueue.ContainsKey(loadingPriority))
                this.loadingQueue.Add(loadingPriority, new Queue<ResourceLoadingTask>());

            Queue<ResourceLoadingTask> queue = this.loadingQueue[loadingPriority];

            int hPrio = Math.Max(loadingPriority, this.loadingQueue.Max(x => x.Value.Count == 0 ? int.MinValue : x.Key));
            lock (this.loadingQueue) {
                queue.Enqueue(loadingTask);
                this.loadingQueueSize++;
                this.queuedResources[resourceIdentifier] = null;

                this.highestPriority = hPrio;
            }
        }

        internal void LoadResource<R, RLP>(string resourceIdentifier, RLP resourceLoadingParameters, int loadingPriority, bool globalResource = false) where RLP : ResourceLoadingParameters<R> {
            if (this.queuedResources.ContainsKey(resourceIdentifier)) {
                Log.WriteLine($"Resource with identifier '{resourceIdentifier}' is already queued for loading.", LogType.Error);
                return;
            }

            ResourceLoadingTask loadingTask = new ResourceLoadingTask(resourceIdentifier, typeof(RLP), resourceLoadingParameters, typeof(R), globalResource);
            if (!this.loadingQueue.ContainsKey(loadingPriority))
                this.loadingQueue.Add(loadingPriority, new Queue<ResourceLoadingTask>());

            Queue<ResourceLoadingTask> queue = this.loadingQueue[loadingPriority];

            int hPrio = Math.Max(loadingPriority, this.loadingQueue.Max(x => x.Value.Count == 0 ? int.MinValue : x.Key));
            lock (this.loadingQueue) {
                queue.Enqueue(loadingTask);
                this.loadingQueueSize++;
                this.queuedResources[resourceIdentifier] = null;

                this.highestPriority = hPrio;
            }
        }

        public void UnloadResource(string resourceIdentifier) {
            if (this.globalResources.ContainsKey(resourceIdentifier))
                this.globalResources.Remove(resourceIdentifier);
        }

        internal void ClearSceneResources() {
            this.sceneResources.Clear();
        }

        public void RegisterResourceLoader<R, RLP>(ResourceLoader<R, RLP> loader) where RLP : ResourceLoadingParameters<R> {
            Type resourceType = typeof(R);
            Type resourceLoadingParameterType = typeof(RLP);

            if (!this.loaders.ContainsKey(resourceType))
                this.loaders[resourceType] = new Dictionary<Type, IResourceLoader>();

            this.loaders[resourceType][resourceLoadingParameterType] = loader;
        }

        private ResourceLoader<R, RLP> GetLoader<R, RLP>() where RLP : ResourceLoadingParameters<R> {
            Type resourceType = typeof(R);
            Type resourceLoadingParameterType = typeof(RLP);

            return (ResourceLoader<R, RLP>)GetLoader(resourceType, resourceLoadingParameterType);
        }

        private IResourceLoader GetLoader(Type resourceType, Type resourceLoadingParameterType) {
            if (!this.loaders.TryGetValue(resourceType, out Dictionary<Type, IResourceLoader> tmp))
                return null;

            if (!tmp.TryGetValue(resourceLoadingParameterType, out IResourceLoader loader))
                return null;

            return loader;
        }

        public bool IsLoading => this.loadingQueueSize > 0;
    }
}