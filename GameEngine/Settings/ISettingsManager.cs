using System;
using System.Collections.Generic;
using GameEngine.Modding;

namespace GameEngine.Settings {
    public interface ISettingsManager {

        #region InstallSettings
        /// <summary>
        /// Installs the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mod">The mod identifier.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="value">The value.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="isReadonly">if set to <c>true</c> [is readonly].</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        void InstallSetting<T>(ModBase mod, string setting, T value, Func<T, string> parser, bool isReadonly, bool isVisible);
        #endregion

        #region GenericSetGet
        /// <summary>
        /// Fetches a base application setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        T Get<T>(string setting, Func<string, T> parser);

        /// <summary>
        /// Fetches a setting.
        /// </summary>
        /// <typeparam name="T">The returned type.</typeparam>
        /// <param name="mod">ID of the mod that the setting belongs to. If null all mods will be searched and the first matching setting will be returned</param>
        /// <param name="setting">The setting's name.</param>
        /// <param name="parser">The parser used to convert from string to T.</param>
        /// <returns>Returns the fetched setting parsed to T.</returns>
        T Get<T>(ModBase mod, string setting, Func<string, T> parser);

        /// <summary>
        /// Sets the setting.
        /// </summary>
        /// <param name="mod">ID of the mod that the setting belongs to.</param>
        /// <param name="setting">The setting's name. Must exist for the mod.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="parser">The parser used to convert from T to string.</param>
        /// <returns>Returns <c>true</c> if the mod and setting were found, the value is not read-only and the value could be parsed; <c>false</c> otherwise.</returns>
        bool Set<T>(ModBase mod, string setting, T value, Func<T, string> parser);

        /// <summary>
        /// Determines whether the specified setting exists.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting exists; otherwise, <c>false</c>.
        /// </returns>
        bool HasSetting(string setting);

        /// <summary>
        /// Determines whether the specified mod has the setting.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified mod has the setting; otherwise, <c>false</c>.
        /// </returns>
        bool HasSetting(ModBase mod, string setting);

        #endregion

        #region Listeners
        /// <summary>
        /// Add a listener for base app setting changes.
        /// </summary>
        /// <param name="listener">The listener. Parameters in order: setting key, new value, old value.</param>
        void AddListener(Action<string, string, string> listener);

        /// <summary>
        /// Add a listener for setting changes.
        /// </summary>
        /// <param name="mod">ID of the mod that the setting belongs to.</param>
        /// <param name="listener">The listener. Parameters in order: modID, setting key, new value, old value.</param>
        void AddListener(ModBase mod, Action<string, string, string, string> listener);
        #endregion

        /// <summary>
        /// Saves the settings.
        /// </summary>
        void SaveSettings();

        #region SpecificSetGet
        #region Int
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        int GetInt(ModBase mod, string setting);

        /// <summary>
        ///<see cref="Set{T}(string,string,x,Func{T,string})"/>
        /// </summary>
        bool SetInt(ModBase mod, string setting, int value);
        #endregion

        #region Float
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        float GetFloat(ModBase mod, string setting);

        /// <summary>
        ///<see cref="Set{T}(string,string,x,Func{T,string})"/>
        /// </summary>
        bool SetFloat(ModBase mod, string setting, float value);
        #endregion

        #region Bool
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        bool GetBool(ModBase mod, string setting);

        /// <summary>
        ///<see cref="Set{T}(string,string,x,Func{T,string})"/>
        /// </summary>
        bool SetBool(ModBase mod, string setting, bool value);
        #endregion

        #region String
        /// <summary>
        ///<see cref="Get{T}(string, Func{string, T}, string)"/>
        /// </summary>
        string GetString(ModBase mod, string setting);

        /// <summary>
        ///<see cref="Set{T}(string,string,x,Func{T,string})"/>
        /// </summary>
        bool SetString(ModBase mod, string setting, string value);
        #endregion
        #endregion

        /// <summary>
        /// Determines whether the specified setting is read-only.
        /// </summary>
        /// <param name="mod">The mod id.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is read-only; otherwise, <c>false</c>.
        /// </returns>
        bool IsReadOnly(ModBase mod, string setting);

        /// <summary>
        /// Determines whether the specified setting is visible.
        /// </summary>
        /// <param name="mod">The mod id.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>
        ///   <c>true</c> if the specified setting is visible; otherwise, <c>false</c>.
        /// </returns>
        bool IsVisible(ModBase mod, string setting);

        /// <summary>
        /// Gets all existing setting keys from the base app.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Settings();

        /// <summary>
        /// Gets all existing setting keys for the specific mod.
        /// </summary>
        /// <param name="mod">ID of the mod the setting keys to be fetched for.</param>
        /// <returns></returns>
        IEnumerable<string> ModSettings(ModBase mod);
    }
}