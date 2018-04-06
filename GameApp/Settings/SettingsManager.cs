using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameApp.Application;
using GameApp.Files;
using GameEngine.Logging;
using GameEngine.Modding;
using GameEngine.Settings;
using SimpleXML;
using Log = GameApp.Logging.Log;
using ModManager = GameApp.Modding.ModManager;

namespace GameApp.Settings {
    internal sealed class SettingsManager : ISettingsManager {

        public const string ATTRIBUTE_NAME_VISIBLE = "visible";
        public const string ATTRIBUTE_NAME_READONLY = "readonly";

        private static SettingsManager instance;
        internal static SettingsManager Instance {
            get => instance;
            private set {
                if (instance != null) throw new InvalidOperationException("Only one instance per manager type permitted.");
                instance = value;
            }
        }

        private XMLElement settings;
        private readonly Dictionary<string, XMLElement> modSettings;

        private readonly List<Action<string, string, string>> settingsListeners;
        private readonly Dictionary<string, List<Action<string, string, string, string>>> modSettingsListeners;

        /// <summary>
        /// Initializes the settings manager. See <see cref="LoadSettings(string, string, string)"/>.
        /// </summary>
        internal SettingsManager() {
            Instance = this;

            this.modSettings = new Dictionary<string, XMLElement>();

            settingsListeners = new List<Action<string, string, string>>();
            modSettingsListeners = new Dictionary<string, List<Action<string, string, string, string>>>();
        }

        internal void Install() {
            settings = new XMLElement(AppConstants.Internals.SETTINGS_XML_ROOT);

            SaveSettings();
        }

        internal bool VerifyInstallation() {
            XMLElement test = FileManager.Instance.LoadFileAsync(XMLReader.Parse, FileManager.Instance.SettingsFile).Result;

            return test != null && test.Tag == AppConstants.Internals.SETTINGS_XML_ROOT;
        }

        internal void Initialize() {
            settings = FileManager.Instance.LoadFileAsync(XMLReader.Parse, FileManager.Instance.SettingsFile).Result;
        }

        internal void LoadModSettings(ModBase mod) {
            string modID = mod.ModID;
            string modSettingsFile = Path.Combine(FileManager.Instance.ModDirectory(mod), mod.SettingsFile);

            if (!File.Exists(modSettingsFile))
                return;

            XMLElement modSettings = FileManager.Instance.LoadFileAsync(XMLReader.Parse, modSettingsFile).Result;
            this.modSettings[modID] = modSettings;
        }

        #region InstallSettings
        internal void InstallSetting<T>(string setting, T value, Func<T, string> parser, bool isReadonly, bool isVisible) {
            XMLElement e = new XMLElement(setting, parser(value));
            e.SetAttribute(ATTRIBUTE_NAME_VISIBLE, isVisible.ToString());
            e.SetAttribute(ATTRIBUTE_NAME_READONLY, isReadonly.ToString());

            settings.AddElement(e);

            SaveSettings();
        }

        public void InstallSetting<T>(ModBase mod, string setting, T value, Func<T, string> parser, bool isReadonly, bool isVisible) {
            string modID = mod.ModID;
            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not set setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                return;
            }

            if (!this.modSettings.ContainsKey(modID)) {
                Log.Instance.WriteLine($"Could not set setting '{setting}' from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                return;
            }

            XMLElement modSettings = this.modSettings[modID];

            XMLElement e = new XMLElement(setting, parser(value));
            e.SetAttribute(ATTRIBUTE_NAME_VISIBLE, isVisible.ToString());
            e.SetAttribute(ATTRIBUTE_NAME_READONLY, isReadonly.ToString());

            modSettings.AddElement(e);

            SaveSettings();
        }
        #endregion

        #region GenericSetGet
        public string this[string setting] {
            get {
                if (!settings.HasElement(setting)) {
                    Log.Instance.WriteLine($"Could not get app setting '{setting}'. Setting does not exist.", LogType.Warning);
                    return null;
                }

                XMLElement settingElement = settings.GetElement(setting);
                if (!settingElement.HasData) {
                    Log.Instance.WriteLine($"Could not get app setting '{setting}'. Not an XML data element.", LogType.Warning);
                    return null;
                }

                return settingElement.Data;
            }
        }

        public string this[ModBase mod, string setting] {
            get {
                string modID = mod.ModID;

                if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                    Log.Instance.WriteLine($"Could not get setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                    return null;
                }

                if (!this.modSettings.ContainsKey(modID)) {
                    Log.Instance.WriteLine($"Could not get setting '{setting}' from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                    return null;
                }

                XMLElement modSettings = this.modSettings[modID];

                if (!modSettings.HasElement(setting)) {
                    Log.Instance.WriteLine($"Could not get setting '{setting}' from mod '{modID}'. Setting does not exist.", LogType.Warning);
                    return null;
                }

                XMLElement settingElement = modSettings.GetElement(setting);
                if (!settingElement.HasData) {
                    Log.Instance.WriteLine($"Could not get setting '{setting}' from mod '{modID}'. Not an XML data element.", LogType.Warning);
                    return null;
                }

                return settingElement.Data;
            }
        }

        /// <summary>
        /// Fetches a base application setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        public T Get<T>(string setting, Func<string, T> parser) {
            string settingData = this[setting];

            if (settingData == null)
                return default(T);

            // parse setting to output
            try {
                return parser(settingData);
            } catch (Exception) {
                Log.Instance.WriteLine($"Could not parse base app setting '{setting}' to type {default(T).GetType().Name}.", LogType.Error);
                return default(T);
            }
        }

        /// <summary>
        /// Fetches a setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="modID">ID of the mod that the setting belongs to. If null all mods will be searched and the first matching setting will be returned</param>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        public T Get<T>(ModBase mod, string setting, Func<string, T> parser) {
            string modID = mod.ModID;
            string settingData = this[mod, setting];

            if (settingData == null)
                return default(T);

            // parse setting to output
            try {
                return parser(settingData);
            } catch (Exception e) {
                Log.Instance.WriteLine($"Could not parse setting '{setting}' from mod '{modID}' to type {default(T).GetType().Name}.", LogType.Error);
                return default(T);
            }
        }

        internal bool Set<T>(string setting, T value, Func<T, string> parser) {
            if (!settings.HasElement(setting)) {
                Log.Instance.WriteLine($"Could not set app setting '{setting}'. Setting does not exist.", LogType.Warning);
                return false;
            }

            XMLElement settingElement = settings.GetElement(setting);
            if (!settingElement.HasData) {
                Log.Instance.WriteLine($"Could not set app setting '{setting}'. Not an XML data element.", LogType.Warning);
                return false;
            }

            bool hasReadonly = settingElement.HasAttribute(ATTRIBUTE_NAME_READONLY);
            bool isReadOnly = true;
            if (hasReadonly && !bool.TryParse(ATTRIBUTE_NAME_READONLY, out isReadOnly)) {
                Log.Instance.WriteLine($"Could not check read-only for app setting '{setting}'. Attribute value could not be parsed.", LogType.Warning);
                return false;
            }

            if (hasReadonly && isReadOnly) {
                Log.Instance.WriteLine($"Could not set app setting '{setting}'. Setting is read-only.", LogType.Warning);
                return false;
            }

            string oldSetting = settingElement.Data;

            // parse setting to output
            string newSetting;
            try {
                newSetting = parser(value);
            } catch (Exception e) {
                Log.Instance.WriteLine($"Could not parse base app setting '{setting}' to type {default(T).GetType().Name}.", LogType.Error);
                return false;
            }
            settingElement.SetData(newSetting);

            // notifiy listeners
            foreach (Action<string, string, string> item in settingsListeners) {
                item(setting, newSetting, oldSetting);
            }

            return true;
        }

        /// <summary>
        /// Sets the setting.
        /// </summary>
        /// <param name="mod">ID of the mod that the setting belongs to.</param>
        /// <param name="setting">The setting's name. Must exist in the setting root.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="parser">The parser used to convert from T to string.</param>
        /// <returns>Returns <c>true</c> if the mod and setting were found, the value is not read-only and the value could be parsed; <c>false</c> otherwise.</returns>
        public bool Set<T>(ModBase mod, string setting, T value, Func<T, string> parser) {
            string modID = mod.ModID;
            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not set setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                return false;
            }

            if (!this.modSettings.ContainsKey(modID)) {
                Log.Instance.WriteLine($"Could not set setting '{setting}' from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                return false;
            }

            XMLElement modSettings = this.modSettings[modID];

            if (!modSettings.HasElement(setting)) {
                Log.Instance.WriteLine($"Could not set setting '{setting}' from mod '{modID}'. Setting does not exist.", LogType.Warning);
                return false;
            }

            XMLElement settingElement = modSettings.GetElement(setting);
            if (!settingElement.HasData) {
                Log.Instance.WriteLine($"Could not get setting '{setting}' from mod '{modID}'. Not an XML data element.", LogType.Warning);
                return false;
            }

            bool hasReadonly = settingElement.HasAttribute(ATTRIBUTE_NAME_READONLY);
            bool isReadOnly = true;
            if (hasReadonly && !bool.TryParse(ATTRIBUTE_NAME_READONLY, out isReadOnly)) {
                Log.Instance.WriteLine($"Could not check read-only for setting '{setting}' from mod '{modID}'. Attribute value could not be parsed.", LogType.Warning);
                return false;
            }

            if (hasReadonly && isReadOnly) {
                Log.Instance.WriteLine($"Could not set app setting '{setting}' from mod '{modID}'. Setting is read-only.", LogType.Warning);
                return false;
            }

            string oldSetting = settingElement.Data;

            // parse setting to output
            string newSetting;
            try {
                newSetting = parser(value);
            } catch (Exception) {
                Log.Instance.WriteLine($"Could not parse base app setting '{setting}' to type {default(T).GetType().Name}.", LogType.Error);
                return false;
            }
            settingElement.SetData(newSetting);

            // notifiy listeners
            if (modSettingsListeners.ContainsKey(modID)) {
                foreach (Action<string, string, string, string> item in modSettingsListeners[modID]) {
                    item(modID, setting, newSetting, oldSetting);
                }
            }

            return true;
        }

        public bool HasSetting(string setting) {
            if (!settings.HasElement(setting))
                return false;

            XMLElement settingElement = settings.GetElement(setting);
            return settingElement.HasData;
        }

        public bool HasSetting(ModBase mod, string setting) {
            string modID = mod.ModID;

            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not get setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                return false;
            }

            if (!this.modSettings.ContainsKey(modID))
                return false;

            XMLElement modSettings = this.modSettings[modID];

            if (!modSettings.HasElement(setting))
                return false;

            XMLElement settingElement = modSettings.GetElement(setting);
            return settingElement.HasData;
        }

        #endregion

        #region Listeners
        public void AddListener(Action<string, string, string> listener) {
            settingsListeners.Add(listener);
        }

        public void AddListener(ModBase mod, Action<string, string, string, string> listener) {
            string modID = mod.ModID;
            if (!modSettingsListeners.ContainsKey(modID))
                modSettingsListeners[modID] = new List<Action<string, string, string, string>>();

            modSettingsListeners[modID].Add(listener);

        }
        #endregion

        public void SaveSettings() {
            FileManager.Instance.SaveFileAsync(xml => XMLWriter.Parse(xml), FileManager.Instance.SettingsFile, settings);

            foreach (KeyValuePair<string, XMLElement> pair in modSettings) {
                string modID = pair.Key;
                string path = Path.Combine(FileManager.Instance.ModDirectory(modID), ModManager.Instance.GetMod(modID).SettingsFile);
                FileManager.Instance.SaveFileAsync(xml => XMLWriter.Parse(xml), path, pair.Value);
            }
        }

        #region SpecificSetGet
        #region Int
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        public int GetInt(ModBase mod, string setting) {
            return Get(mod, setting, int.Parse);
        }

        /// <summary>
        ///<see cref="Set{T}(string, T, Func{T, string}, string)"/>
        /// </summary>
        public bool SetInt(ModBase mod, string setting, int value) {
            return Set(mod, setting, value, x => value.ToString());
        }
        #endregion

        #region Float
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        public float GetFloat(ModBase mod, string setting) {
            return Get(mod, setting, float.Parse);
        }

        /// <summary>
        ///<see cref="Set{T}(string, T, Func{T, string}, string)"/>
        /// </summary>
        public bool SetFloat(ModBase mod, string setting, float value) {
            return Set(mod, setting, value, x => value.ToString());
        }
        #endregion

        #region Bool
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        public bool GetBool(ModBase mod, string setting) {
            return Get(mod, setting, bool.Parse);
        }

        /// <summary>
        ///<see cref="Set{T}(string, T, Func{T, string}, string)"/>
        /// </summary>
        public bool SetBool(ModBase mod, string setting, bool value) {
            return Set(mod, setting, value, x => value.ToString());
        }
        #endregion

        #region String
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        public string GetString(ModBase mod, string setting) {
            return Get(mod, setting, x => x);
        }

        /// <summary>
        ///<see cref="Set{T}(string, T, Func{T, string}, string)"/>
        /// </summary>
        public bool SetString(ModBase mod, string setting, string value) {
            return Set(mod, setting, value, x => x);
        }
        #endregion
        #endregion

        /// <summary>
        /// Determines whether the specified setting is read-only.
        /// </summary>
        /// <param name="modID">The mod id.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is read-only; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReadOnly(ModBase mod, string setting) {
            string modID = mod.ModID;
            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not check read-only for setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                return false;
            }

            if (!this.modSettings.ContainsKey(modID)) {
                Log.Instance.WriteLine($"Could not check read-only setting '{setting}' from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                return false;
            }

            XMLElement modSettings = this.modSettings[modID];

            if (!modSettings.HasElement(setting)) {
                Log.Instance.WriteLine($"Could not check read-only setting '{setting}' from mod '{modID}'. Setting does not exist.", LogType.Warning);
                return false;
            }

            XMLElement settingElement = modSettings.GetElement(setting);
            if (!settingElement.HasData) {
                Log.Instance.WriteLine($"Could not check read-only setting '{setting}' from mod '{modID}'. Not an XML data element.", LogType.Warning);
                return false;
            }

            bool hasAttrib = settingElement.HasAttribute(ATTRIBUTE_NAME_READONLY);
            bool attribValue = true;
            if (hasAttrib && !bool.TryParse(ATTRIBUTE_NAME_READONLY, out attribValue)) {
                Log.Instance.WriteLine($"Could not check read-only for setting '{setting}'. Attribute value could not be parsed.", LogType.Warning);
                return false;
            }

            return hasAttrib && attribValue;
        }

        /// <summary>
        /// Determines whether the specified setting is visible.
        /// </summary>
        /// <param name="modID">The mod id.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVisible(ModBase mod, string setting) {
            string modID = mod.ModID;
            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not check visibility for setting '{setting}'. Invalid mod id '{modID}'.", LogType.Warning);
                return false;
            }

            if (!this.modSettings.ContainsKey(modID)) {
                Log.Instance.WriteLine($"Could not check visibility setting '{setting}' from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                return false;
            }

            XMLElement modSettings = this.modSettings[modID];

            if (!modSettings.HasElement(setting)) {
                Log.Instance.WriteLine($"Could not check visibility setting '{setting}' from mod '{modID}'. Setting does not exist.", LogType.Warning);
                return false;
            }

            XMLElement settingElement = modSettings.GetElement(setting);
            if (!settingElement.HasData) {
                Log.Instance.WriteLine($"Could not check visibility setting '{setting}' from mod '{modID}'. Not an XML data element.", LogType.Warning);
                return false;
            }

            bool hasAttrib = settingElement.HasAttribute(ATTRIBUTE_NAME_VISIBLE);
            bool attribValue = true;
            if (hasAttrib && !bool.TryParse(ATTRIBUTE_NAME_VISIBLE, out attribValue)) {
                Log.Instance.WriteLine($"Could not check visibility for setting '{setting}'. Attribute value could not be parsed.", LogType.Warning);
                return false;
            }

            return hasAttrib && attribValue;
        }

        public IEnumerable<string> Settings() {
            return settings.NestedElements.Select(e => e.Tag);
        }

        public IEnumerable<string> ModSettings(ModBase mod) {
            string modID = mod.ModID;
            if (string.IsNullOrWhiteSpace(modID) || modID.Length < AppConstants.Mods.MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Could not get settings. Invalid mod id '{modID}'.", LogType.Warning);
                return new List<string>();
            }

            if (!this.modSettings.ContainsKey(modID)) {
                Log.Instance.WriteLine($"Could not get settings from mod '{modID}'. Mod does not exist or has not settings file.", LogType.Warning);
                return new List<string>();
            }

            XMLElement modSettings = this.modSettings[modID];

            return modSettings.NestedElements.Select(e => e.Tag);
        }

    }
}