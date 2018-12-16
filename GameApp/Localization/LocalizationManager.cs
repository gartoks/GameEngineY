using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameApp.Application;
using GameApp.Settings;
using GameEngine.Localization;
using GameEngine.Logging;
using GameEngine.Modding;
using SimpleINI;
using Log = GameApp.Logging.Log;

namespace GameApp.Localization {
    internal sealed class LocalizationManager : ILocalizationManager {

        private static LocalizationManager instance;
        internal static LocalizationManager Instance {
            get => LocalizationManager.instance;
            private set { if (LocalizationManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private string defaultLanguage;

        private string currentLanguage;
        private readonly Dictionary<string, string> languageData;

        /// <summary>
        /// Initializes the localization handler.
        /// </summary>
        internal LocalizationManager() {
            Instance = this;

            this.languageData = new Dictionary<string, string>();
        }

        internal void Install() {
            SettingsManager.Instance.InstallSetting(AppConstants.SettingKeys.APP_LANGUAGE, AppConstants.Defaults.APP_LANGUAGE, x => x, false, true);
        }

        internal bool VerifyInstallation() => SettingsManager.Instance.HasSetting(AppConstants.SettingKeys.APP_LANGUAGE);

        internal void Initialize(string defaultLanguage) {
            this.defaultLanguage = defaultLanguage;

            if (string.IsNullOrEmpty(defaultLanguage))
                throw new ArgumentException("A default language must be specified.", nameof(defaultLanguage));

            string language = SettingsManager.Instance[AppConstants.SettingKeys.APP_LANGUAGE];

            if (string.IsNullOrWhiteSpace(language))
                language = defaultLanguage;

            SetLanguage(language);
        }

        /// <inheritdoc />
        /// <summary>
        /// Localizes the specified key to the currently loaded language see <see cref="P:GameApp.Localization.LocalizationManager.CurrentLanguage" />. If the key could not be found the key itself is returned and a warning is logged.
        /// </summary>
        /// <param name="localizationKey">The localization key.</param>
        /// <returns>Returns the associated, localized text to the key. </returns>
        public string Localize(string localizationKey) {
            if (!this.languageData.TryGetValue(localizationKey, out string localized)) {
                Log.Instance.WriteLine($"Localization key '{localizationKey}' could not be found.", LogType.Warning);
                localized = localizationKey;
            }

            return localized;
        }

        /// <summary>
        /// Loads and sets the language given if available (see <see cref="IsLanguageAvailable(string)"/>).
        /// </summary>
        /// <param name="language">The language to be loaded.</param>
        public void SetLanguage(string language) {
            if (this.currentLanguage != null && this.currentLanguage.Equals(language))
                return;

            if (string.IsNullOrEmpty(language)) {
                Log.Instance.WriteLine("Language could not be found.", LogType.Warning);
                return;
            }

            this.currentLanguage = language;

            LoadLanguage(this.currentLanguage);
        }

        /// <summary>
        /// Resets the language to the default language.
        /// </summary>
        public void ResetLanguage() {
            SetLanguage(this.defaultLanguage);
        }

        /// <summary>
        /// Returns the currently loaded language.
        /// </summary>
        public string CurrentLanguage {
            get => this.currentLanguage;
            set => SetLanguage(value);
        }

        internal void ReloadLanguage() => LoadLanguage(CurrentLanguage);

        /// <summary>
        /// Loads the language. Assumes the language exists.
        /// </summary>
        /// <param name="language">The language to be loaded.</param>
        private void LoadLanguage(string language) {
            // prepare
            this.languageData.Clear();

            foreach (ModBase mod in Modding.ModManager.Instance.Mods) {
                Modding.ModManager.Instance.GetModDirectory(mod.ModID, out string modDir);
                string modLocalizationDir = Path.Combine(modDir, mod.LocalizationDirectory);

                if (!Directory.Exists(modLocalizationDir))
                    continue;

                IEnumerable<string> langFiles = Directory.EnumerateFiles(modLocalizationDir).Where(f => Path.GetFileName(f).Equals(language + "." + AppConstants.FileExtensions.LOCALIZATION_FILE));
                if (!langFiles.Any())
                    continue;

                if (langFiles.Count() > 1) {
                    Log.Instance.WriteLine($"Ambiguous language files found for mod '{mod.Name}' at {modLocalizationDir}.");
                    continue;
                }

                string languageFile = langFiles.Single();
                foreach (KeyValuePair<string, string> languagePair in INIReader.Read(languageFile).GetValues())
                    this.languageData[$"{mod.ModID}:{languagePair.Key}"] = languagePair.Value.Trim();

                //foreach (string line in File.ReadLines(languageFile)) {
                //    string[] data = line.Trim().Split('=');

                //    // format: key=text
                //    if (data.Length != 2) {
                //        Log.Instance.WriteLine("Could not parse language line.", LogType.Warning);
                //        continue;
                //    }

                //    this.languageData[$"{mod.ModID}:{data[0].Trim()}"] = data[1].Trim();
                //}
            }

        }

    }
}