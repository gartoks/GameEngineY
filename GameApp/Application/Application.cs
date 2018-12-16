using System;
using System.Threading;
using GameApp.Files;
using GameApp.Game;
using GameApp.Graphics;
using GameApp.Input;
using GameApp.Localization;
using GameApp.Modding;
using GameApp.Resources;
using GameApp.Settings;
using GameApp.Utility;
using GameEngine.Application;
using GameEngine.Exceptions;
using GameEngine.Logging;
using GameEngine.Utility.Extensions;
using Log = GameApp.Logging.Log;
using Version = GameEngine.Utility.Version;
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable EmptyGeneralCatchClause

namespace GameApp.Application {
    public sealed class Application : IApplication {
        public static Application Instance {
            get => Application.instance;
            set {
                if (Application.instance != null)
                    throw new InvalidOperationException("Only one instance of App can exist at a time.");

                Application.instance = value;
            }
        }

        private static Application instance;

        private readonly ApplicationInitializationSettings initializationSettings;

        public string BaseModID => initializationSettings.BaseModID;
        public string Name => initializationSettings.Name;
        public Version Version => initializationSettings.Version;

        private bool running;
        internal Thread UpdateThread { get; private set; }
        private Thread resourceThread;
        internal Thread RenderThread => Window.Window.Instance.RenderThread;

        public Application(ApplicationInitializationSettings initializationSettings) {
            Instance = this;

            this.initializationSettings = initializationSettings ?? throw new ArgumentNullException(nameof(initializationSettings));

            ConsoleActivator.ShowConsole = initializationSettings.ShowConsole;

            new Log();
            Log.Instance.OnLog += (text, type, color) => Console.WriteLine($"{(type == LogType.Message ? "" : "[" + type.ToString().PadBoth(7) + "]")}" + $"{text}");

            SetupManagers();
        }

        public void Launch() {
            if (!IsInstalled)
                Install();
            else
                Initialize();

            if (!VerifyInstallation())
                throw new ProgramException("App installation is corrupt. Reinstall app.");


            Application.Instance.resourceThread = new Thread(ResourceLoop);
            Application.instance.resourceThread.Name = "ResourceThread";
            Application.Instance.UpdateThread = new Thread(UpdateLoop);
            Application.instance.UpdateThread.Name = "UpdateThread";

            Application.Instance.running = true;

            Application.Instance.resourceThread.Start();
            Application.Instance.UpdateThread.Start();
            Window.Window.Instance.Show();
        }

        public void Shutdown() {
            Thread exitThread = new Thread(() => {
                this.running = false;
                try {
                    resourceThread.Join();
                    UpdateThread.Join();

                    Window.Window.Instance.Close();
                }
                catch (Exception) { }
            });

            exitThread.Start();
        }

        private static void SetupManagers() {
            new FileManager();
            new SettingsManager();
            new ResourceManager();
            new LocalizationManager();
            new Window.Window();
            new GLHandler();
            new GraphicsHandler();
            new InputManager();
            new TimeManager();
            new SceneManager();
            new ModManager();
        }

        private static void Install() {
            ApplicationInitializationSettings initializationSettings = Instance.initializationSettings;

            FileManager.Instance.Install();
            FileManager.Instance.Initialize();

            SettingsManager.Instance.Install();
            SettingsManager.Instance.Initialize();

            ResourceManager.Instance.Install();
            ResourceManager.Instance.Initialize();

            LocalizationManager.Instance.Install();
            LocalizationManager.Instance.Initialize(initializationSettings.DefaultLanguage);

            Window.Window.Instance.Install();
            Window.Window.Instance.Initialize(initializationSettings.Name, initializationSettings.Icon);

            GLHandler.Instance.Install();
            GLHandler.Instance.Initialize();

            GraphicsHandler.Instance.Install();
            GraphicsHandler.Instance.Initialize();

            InputManager.Instance.Install();
            InputManager.Instance.Initialize();

            TimeManager.Instance.Install();
            TimeManager.Instance.Initialize(initializationSettings.IsSimulation);

            SceneManager.Instance.Install();
            SceneManager.Instance.Initialize();

            ModManager.Instance.Install();
            ModManager.Instance.Initialize();

            RegistryHandler.Install(Application.instance.initializationSettings.Name, Application.instance.initializationSettings.Version);
        }

        private static bool VerifyInstallation() {
            return
                FileManager.Instance.VerifyInstallation() &&
                SettingsManager.Instance.VerifyInstallation() &&
                ResourceManager.Instance.VerifyInstallation() &&
                LocalizationManager.Instance.VerifyInstallation() &&
                Window.Window.Instance.VerifyInstallation() &&
                GLHandler.Instance.VerifyInstallation() &&
                GraphicsHandler.Instance.VerifyInstallation() &&
                InputManager.Instance.VerifyInstallation() &&
                TimeManager.Instance.VerifyInstallation() &&
                SceneManager.Instance.VerifyInstallation() &&
                ModManager.Instance.VerifyInstallation();
        }

        private static void Initialize() {
            ApplicationInitializationSettings initializationSettings = Instance.initializationSettings;

            FileManager.Instance.Initialize();
            SettingsManager.Instance.Initialize();
            LocalizationManager.Instance.Initialize(initializationSettings.DefaultLanguage);
            ResourceManager.Instance.Initialize();
            Window.Window.Instance.Initialize(initializationSettings.Name, initializationSettings.Icon);
            GLHandler.Instance.Initialize();
            GraphicsHandler.Instance.Initialize();
            InputManager.Instance.Initialize();
            TimeManager.Instance.Initialize(initializationSettings.IsSimulation);
            SceneManager.Instance.Initialize();
            ModManager.Instance.Initialize();
        }

        private static void UpdateLoop() {
            ModManager.Instance.InitializeMods();

            SceneManager.Instance.TryLoadDefaultScene();

            while (Application.Instance.running) {
                TimeManager.Instance.Tick(() => {
                    InputManager.Instance.Update();
                    SceneManager.Instance.Update();
                });
            }
        }

        private static void ResourceLoop() {
            while (Application.Instance.running) {
                if (ResourceManager.Instance.IsLoading)
                    ResourceManager.Instance.ContinueLoading();
                else
                    Thread.Sleep(AppConstants.Internals.RESOURCE_THREAD_IDLE_SLEEP_TIME);
            }
        }

        public static bool IsInstalled => RegistryHandler.IsInstalled(Application.instance.initializationSettings.Name, Application.instance.initializationSettings.Version);

    }
}