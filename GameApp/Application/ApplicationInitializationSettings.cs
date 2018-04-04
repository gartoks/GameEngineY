using System;
using Version = GameEngine.Utility.Version;

namespace GameApp.Application {
    public class ApplicationInitializationSettings {
        private string name;
        private Version version;
        private string defaultLanguage;

        public bool IsSimulation = false;
        public System.Drawing.Icon Icon = null;

        public ApplicationInitializationSettings(string name, Version version, string defaultLanguage) {
            Name = name;
            Version = version;
            DefaultLanguage = defaultLanguage;
        }

        internal ApplicationInitializationSettings(ApplicationInitializationSettings ais)
            : this(ais.Name, ais.Version, ais.DefaultLanguage) {

            Icon = ais.Icon;
        }

        public string Name {
            get => this.name;
            set {
                if (string.IsNullOrWhiteSpace(name))
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
                if (string.IsNullOrWhiteSpace(defaultLanguage))
                    throw new ArgumentNullException(nameof(value));

                this.defaultLanguage = value;
            }
        }
    }
}