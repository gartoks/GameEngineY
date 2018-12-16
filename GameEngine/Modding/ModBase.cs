using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Application;
using GameEngine.Files;
using GameEngine.Game;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.Localization;
using GameEngine.Logging;
using GameEngine.Resources;
using GameEngine.Settings;
using GameEngine.Utility.Extensions;
using GameEngine.Window;

namespace GameEngine.Modding {
    /// <summary>
    /// Every mod has to have exactly one class in the namespace called the same as 'Name' named 'Mod' extending from this class.
    /// </summary>
    public abstract class ModBase {

        private static ModBase instance;

        public static ModBase Instance {
            get => instance;
            private set {
                if (ModBase.instance != null)
                    throw new InvalidOperationException("Only one instance per mod type permitted.");

                ModBase.instance = value;
            }
        }

        public static bool IsBaseMod { get; private set; }

        private bool loaded;
        private IApplication app;
        private ILog log;
        private IFileManager fileManager;
        private ISettingsManager settingsManager;
        private IResourceManager resourceManager;
        private ILocalizationManager localizationManager;
        private IWindow window;
        private IGraphicsHandler graphicsHandler;
        private IInputManager inputManager;
        private ITimeManager timeManager;
        private ISceneManager sceneManager;
        private IModManager modManager;

        private readonly Dictionary<string, object> customModData;

        protected ModBase() {
            Instance = this;
            this.loaded = false;

            this.customModData = new Dictionary<string, object>();
        }

        public void OnLoad(bool isBaseMod,
            IApplication app, ILog log, IFileManager fileManager, ISettingsManager settingsManager,
            IResourceManager resourceManager, ILocalizationManager localizationManager, IWindow window,
            IGraphicsHandler graphicsHandler, IInputManager inputManager, ITimeManager timeManager,
            ISceneManager sceneManager, IModManager modManager) {

            log.OnLog += Logging.Log.InvokeEvent;

            if (this.loaded) {
                log.WriteLine(this, $"Mod '{ModID}' is already loaded.", LogType.Warning);
                return;
            }

            IsBaseMod = isBaseMod;

            this.loaded = true;

            this.app = app;
            this.log = log;
            this.fileManager = fileManager;
            this.settingsManager = settingsManager;
            this.resourceManager = resourceManager;
            this.localizationManager = localizationManager;
            this.window = window;
            this.graphicsHandler = graphicsHandler;
            this.inputManager = inputManager;
            this.timeManager = timeManager;
            this.sceneManager = sceneManager;
            this.modManager = modManager;
        }

        public void Shutdown() => App.Shutdown();

        /// <summary>
        /// Sets a custom mod data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="customData">The custom data.</param>
        public void SetCustomData(string key, object customData) {
            this.customModData[key] = customData;
        }

        /// <summary>
        /// Gets the custom data with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the custom data, null if no data is associated with the given key.</returns>
        public object GetCustomData(string key) {
            if (!this.customModData.TryGetValue(key, out object value))
                return null;
            else
                return value;
        }

        /// <summary>
        /// Gets the mod's custom data.
        /// </summary>
        /// <value>
        /// The mod's custom data. Should return an empty <see>
        ///         <cref>IEnumerable{(string identifier, object data)}</cref>
        ///     </see>
        ///     if there is none.
        /// </value>
        public IEnumerable<(string key, object data)> CustomModData => this.customModData.Select(pair => pair.ToTuple());

        /// <summary>
        /// Gets the mod's directory.
        /// </summary>
        /// <value>
        /// The mod directory.
        /// </value>
        public string ModDirectory => FileManager.ModDirectory(this);

        /// <summary>
        /// Gets the app.
        /// </summary>
        /// <value>
        /// The app.
        /// </value>
        internal static IApplication App => Instance.app;

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        internal static ILog Log => Instance.log;

        /// <summary>
        /// Gets the game window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        internal static IWindow Window => Instance.window;

        /// <summary>
        /// Gets the file manager.
        /// </summary>
        /// <value>
        /// The file manager.
        /// </value>
        internal static IFileManager FileManager => Instance.fileManager;

        /// <summary>
        /// Gets the settings manager.
        /// </summary>
        /// <value>
        /// The settings manager.
        /// </value>
        internal static ISettingsManager SettingsManager => Instance.settingsManager;

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <value>
        /// The resource manager.
        /// </value>
        internal static IResourceManager ResourceManager => Instance.resourceManager;

        /// <summary>
        /// Gets the localization manager.
        /// </summary>
        /// <value>
        /// The localization manager.
        /// </value>
        internal static ILocalizationManager LocalizationManager => Instance.localizationManager;

        /// <summary>
        /// Gets the mod manager.
        /// </summary>
        /// <value>
        /// The mod manager.
        /// </value>
        internal static IModManager ModManager => Instance.modManager;

        /// <summary>
        /// Gets the time manager.
        /// </summary>
        /// <value>
        /// The time manager.
        /// </value>
        internal static ITimeManager TimeManager => Instance.timeManager;

        /// <summary>
        /// Gets the input manager.
        /// </summary>
        /// <value>
        /// The input manager.
        /// </value>
        internal static IInputManager InputManager => Instance.inputManager;

        /// <summary>
        /// Gets the scene manager.
        /// </summary>
        /// <value>
        /// The scene manager.
        /// </value>
        internal static ISceneManager SceneManager => Instance.sceneManager;

        /// <summary>
        /// Gets the OpenGL handler.
        /// </summary>
        /// <value>
        /// The OpenGL handler.
        /// </value>
        internal static IGraphicsHandler GraphicsHandler => Instance.graphicsHandler;

        /// <summary>
        /// This method should be used to check for required files,
        /// localization keys should be registered here,
        /// settings can be changed depoending on startup,
        /// the availability of mods can be checked.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Called when a scene is loaded.
        /// </summary>
        /// <param name="sceneName">Name of the scene.</param>
        public abstract void OnSceneLoad(string sceneName);

        /// <summary>
        /// Gets the human-readable display name of the mod.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the mod identifier. Must be unique to every mod. If a mod attempts to load with an already existing id, it will not be loaded. Must begin with a letter (a-z, A-Z) at be at least 3 characters long.
        /// </summary>
        public abstract string ModID { get; }

        /// <summary>
        /// Gets the mod's loading priority. The smaller, the earlier the mod will be loaded.
        /// If multiple mods share the same priority the loading order between them cannot be relied upon.
        /// Minimum priority is 1.
        /// </summary>
        public abstract int ModLoadingPriority { get; }

        ///// <summary>
        ///// Is called repeatedly to update the game. 
        ///// </summary>
        //public virtual void Update() { }

        ///// <summary>
        ///// Called when a major event in the mods life cycle occurs.
        ///// </summary>
        ///// <param name="e">The event.</param>
        //void OnEvent(ModEvent e);

        /// <summary>
        /// Gets the default settings file for this mod. Will only be called when the mod is added for the first time. Return null if no settings are needed.
        /// </summary>
        /// <value>
        /// The default settings file.
        /// </value>
        public abstract string SettingsFile { get; }

        /// <summary>
        /// Gets the mod's localization directory file.
        /// </summary>
        /// <value>
        /// The localization directory file. Should return null if no localization directory exists.
        /// </value>
        public abstract string LocalizationDirectory { get; }
    }
}