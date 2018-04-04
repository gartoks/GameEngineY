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
using Log = GameApp.Logging.Log;
using Version = GameEngine.Utility.Version;

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


        public string Name => initializationSettings.Name;
        public Version Version => initializationSettings.Version;

        private bool running = false;
        public Thread UpdateThread { get; private set; }
        public Thread ResourceThread { get; private set; }

        public Application(ApplicationInitializationSettings initializationSettings) {
            Instance = this;

            this.initializationSettings = initializationSettings ?? throw new ArgumentNullException(nameof(initializationSettings));

            new Log();

            SetupManagers();
        }

        public void Launch() {
            if (!IsInstalled)
                Install();
            else if (!VerifyInstallation())
                throw new ProgramException("App installation is corrupt. Reinstall app.");

            Initialize();

            Application.Instance.ResourceThread = new Thread(ResourceLoop);
            Application.instance.ResourceThread.Name = "ResourceThread";
            Application.Instance.UpdateThread = new Thread(UpdateLoop);
            Application.instance.UpdateThread.Name = "UpdateThread";

            Application.Instance.running = true;

            Window.Window.Instance.Show();
            Application.Instance.ResourceThread.Start();
            Application.Instance.UpdateThread.Start();
        }

        public void Shutdown() {
            Thread exitThread = new Thread(() => {
                this.running = false;
                try {
                    ResourceThread.Join();
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
            new InputManager();
            new TimeManager();
            new SceneManager();
            new ModManager();
        }

        private static void Install() {
            //SettingsManager.Instance.InstallSetting(AppConstants.SettingKeys.APP_UPDATE_RATE, 60, x => x.ToString(), true, false);
            //SettingsManager.Instance.InstallSetting(AppConstants.SettingKeys.APP_RENDER_RATE, 0, x => x.ToString(), true, false);
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
            Window.Window.Instance.Initialize(initializationSettings.Icon);

            GLHandler.Instance.Install();
            GLHandler.Instance.Initialize();

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
            Window.Window.Instance.Initialize(initializationSettings.Icon);
            GLHandler.Instance.Initialize();
            InputManager.Instance.Initialize();
            TimeManager.Instance.Initialize(initializationSettings.IsSimulation);
            SceneManager.Instance.Initialize();
            ModManager.Instance.Initialize();
        }

        private static void UpdateLoop() {
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