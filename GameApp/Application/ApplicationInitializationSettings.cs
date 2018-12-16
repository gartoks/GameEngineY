using System;
using Version = GameEngine.Utility.Version;

namespace GameApp.Application {
    public class ApplicationInitializationSettings {
        private string baseModID;
        private string name;
        private Version version;
        private string defaultLanguage;

        public bool IsSimulation = false;
        public System.Drawing.Icon Icon;
        public bool ShowConsole = true;

        public ApplicationInitializationSettings(string baseModID, string name, Version version, string defaultLanguage) {
            BaseModID = baseModID;
            Name = name;
            Version = version;
            DefaultLanguage = defaultLanguage;
        }

        internal ApplicationInitializationSettings(ApplicationInitializationSettings ais)
            : this(ais.BaseModID, ais.Name, ais.Version, ais.DefaultLanguage) {

            Icon = ais.Icon;
        }

        public string BaseModID {
            get => this.baseModID;
            private set {
                if (string.IsNullOrWhiteSpace(value) || value.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH)
                    throw new ArgumentNullException(nameof(value));

                this.baseModID = value;
            }
        }

        public string Name {
            get => this.name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                this.name = value;
            }
        }

        public Version Version {
            get => this.version;
            set => this.version = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string DefaultLanguage {
            get => this.defaultLanguage;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                this.defaultLanguage = value;
            }
        }
    }
}