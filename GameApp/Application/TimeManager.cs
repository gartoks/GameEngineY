using System;
using GameApp.Settings;
using GameApp.Utility;
using GameEngine.Application;

namespace GameApp.Application {
    internal sealed class TimeManager : ITimeManager {

        private static TimeManager instance;
        internal static TimeManager Instance {
            get => TimeManager.instance;
            private set { if (TimeManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        internal bool IsSimulation { get; private set; }
        private TimeTracker timeTracker;

        internal TimeManager() {
            Instance = this;
        }

        internal void Install() {
            SettingsManager.Instance.InstallSetting(AppConstants.SettingKeys.APP_UPDATES_PER_SECOND, AppConstants.Defaults.APP_UPDATES_PER_SECOND, x => x.ToString(), false, true);
        }

        internal bool VerifyInstallation() => SettingsManager.Instance.HasSetting(AppConstants.SettingKeys.APP_UPDATES_PER_SECOND);

        internal void Initialize(bool isSimulation) {
            IsSimulation = isSimulation;
            this.timeTracker = new TimeTracker(SettingsManager.Instance.Get(AppConstants.SettingKeys.APP_UPDATES_PER_SECOND, int.Parse));

            SettingsManager.Instance.AddListener((setting, newSetting, oldSetting) => {
                if (setting == AppConstants.SettingKeys.APP_UPDATES_PER_SECOND)
                    this.timeTracker.TargetsTicksPerSecond = int.Parse(newSetting);
            });
        }

        internal void Tick(Action tickAction) {
            this.timeTracker.FullTick(dT => tickAction?.Invoke(), !IsSimulation);
        }

        public float DeltaTime => this.timeTracker.DeltaTime;

        public float TimeSinceStart => this.timeTracker.RunTimeSeconds();
    }
}