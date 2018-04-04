using System;
using System.Collections.Generic;
using GameEngine.Modding;

namespace GameEngine.Settings {
    public static class SettingsManager {

        #region InstallSetting
        /// <summary>
        /// Installs the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setting">The setting.</param>
        /// <param name="value">The value.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="isReadonly">if set to <c>true</c> [is readonly].</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        public static void InstallSetting<T>(string setting, T value, Func<T, string> parser, bool isReadonly, bool isVisible) {
            ModBase.SettingsManager.InstallSetting(ModBase.Instance, setting, value, parser, isReadonly, isVisible);
        }
        #endregion

        #region GenericSetGet
        /// <summary>
        /// Fetches a base application setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        public static T GetAppSetting<T>(string setting, Func<string, T> parser) {
            return ModBase.SettingsManager.Get(setting, parser);
        }

        /// <summary>
        /// Fetches a setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        public static T Get<T>(string setting, Func<string, T> parser) {
            return ModBase.SettingsManager.Get(ModBase.Instance, setting, parser);
        }

        /// <summary>
        /// Sets the setting.
        /// </summary>
        /// <param name="setting">The setting's name. Must exist for the mod.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="parser">The parser used to convert from public static T to string.</param>
        /// <returns>Returns <c>true</c> if the mod and setting were found, the value is not read-only and the value could be parsed; <c>false</c> otherwise.</returns>
        public static bool Set<T>(string setting, T value, Func<T, string> parser) {
            return ModBase.SettingsManager.Set(ModBase.Instance, setting, value, parser);
        }

        /// <summary>
        /// Determines whether the specified setting exists.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting exists; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasSetting(string setting) => ModBase.SettingsManager.HasSetting(ModBase.Instance, setting);

        #endregion

        #region Listeners
        /// <summary>
        /// Add a listener for base app setting changes.
        /// </summary>
        /// <param name="listener">The listener. Parameters in order: setting key, new value, old value.</param>
        public static void AddAppSettingsListener(Action<string, string, string> listener) {
            ModBase.SettingsManager.AddListener(listener);
        }

        /// <summary>
        /// Add a listener for setting changes.
        /// </summary>
        /// <param name="listener">The listener. Parameters in order: modID, setting key, new value, old value.</param>
        public static void AddSettingsListener(Action<string, string, string, string> listener) {
            ModBase.SettingsManager.AddListener(ModBase.Instance, listener);
        }
        #endregion

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void SaveSettings() {
            ModBase.SettingsManager.SaveSettings();
        }

        #region SpecificSetGet
        #region Int
        /// <summary>
        ///<see cref="Get{T}(string,Func{string,T}(string))"/>
        /// </summary>
        public static int GetInt(string setting) {
            return ModBase.SettingsManager.GetInt(ModBase.Instance, setting);
        }

        /// <summary>
        ///<see cref="Set{T}(string,x,Func{T,string})"/>
        /// </summary>
        public static bool SetInt(string setting, int value) {
            return ModBase.SettingsManager.SetInt(ModBase.Instance, setting, value);
        }
        #endregion

        #region Float
        /// <summary>
        ///<see cref="Get{T}(string,System.Func{string,T}(string))"/>
        /// </summary>
        public static float GetFloat(string modID, string setting) {
            return ModBase.SettingsManager.GetFloat(ModBase.Instance, setting);
        }

        /// <summary>
        ///<see cref="Set{T}(string,x,Func{T,string})"/>
        /// </summary>
        public static bool SetFloat(string setting, float value) {
            return ModBase.SettingsManager.SetFloat(ModBase.Instance, setting, value);
        }
        #endregion

        #region Bool
        /// <summary>
        ///<see cref="Get{T}(string,Func{string,T}(string))"/>
        /// </summary>
        public static bool GetBool(string modID, string setting) {
            return ModBase.SettingsManager.GetBool(ModBase.Instance, setting);
        }

        /// <summary>
        ///<see cref="Set{T}(string,x,Func{T,string})"/>
        /// </summary>
        public static bool SetBool(string setting, bool value) {
            return ModBase.SettingsManager.SetBool(ModBase.Instance, setting, value);
        }
        #endregion

        #region String
        /// <summary>
        ///<see cref="Get{T}(string,System.Func{string,T}(string))"/>
        /// </summary>
        public static string GetString(string modID, string setting) {
            return ModBase.SettingsManager.GetString(ModBase.Instance, setting);
        }

        /// <summary>
        ///<see cref="Set{T}(string,x,Func{T,string})"/>
        /// </summary>
        public static bool SetString(string setting, string value) {
            return ModBase.SettingsManager.SetString(ModBase.Instance, setting, value);
        }
        #endregion
        #endregion

        /// <summary>
        /// Determines whether the specified setting is read-only.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is read-only; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReadOnly(string setting) {
            return ModBase.SettingsManager.IsReadOnly(ModBase.Instance, setting);
        }

        /// <summary>
        /// Determines whether the specified setting is visible.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is visible; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVisible(string setting) {
            return ModBase.SettingsManager.IsVisible(ModBase.Instance, setting);
        }

        /// <summary>
        /// Gets all existing setting keys from the base app.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> AppSettings => ModBase.SettingsManager.Settings();

        /// <summary>
        /// Gets the mod's settings.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Settings => ModBase.SettingsManager.ModSettings(ModBase.Instance);
    }
}