using System;
using GameEngine.Logging;
using GameEngine.Resources;

namespace GameApp.Resources {
    internal class ResourceLoadingTask {

        public readonly string ResourceIdentifier;
        public readonly Type ResourceLoadingParametersType;
        public readonly IResourceLoadingParameters ResourceLoadingParameters;
        public readonly Type ResourceType;
        public readonly bool GlobalResource;

        internal ResourceLoadingTask(string resourceIdentifier, Type resourceLoadingParametersType, IResourceLoadingParameters resourceLoadingParameters, Type resourceType, bool globalResource) {
            if (string.IsNullOrEmpty(resourceIdentifier)) {
                Log.WriteLine("Invalid resource identifier.", LogType.Error);
                return;
            }

            ResourceIdentifier = resourceIdentifier;
            ResourceLoadingParametersType = resourceLoadingParametersType;
            ResourceLoadingParameters = resourceLoadingParameters;
            ResourceType = resourceType;
            GlobalResource = globalResource;
        }
    }
}