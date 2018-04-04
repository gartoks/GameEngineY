using GameEngine.Modding;

namespace GameEngine.Localization {
    public static class Localization {
        /// <summary>
        /// Localizes the specified key to the currently loaded language see <see cref="CurrentLanguage"/>. If the key could not be found the key itself is returned and a warning is logged.
        /// </summary>
        /// <param name="localizationKey">The localization key.</param>
        /// <returns>Returns the associated, localized text to the key. </returns>
        public static string Localize(string localizationKey) => ModBase.LocalizationManager.Localize(localizationKey);

        /// <summary>
        /// Returns the currently loaded language.
        /// </summary>
        public static string CurrentLanguage {
            get => ModBase.LocalizationManager.CurrentLanguage;
            set => ModBase.LocalizationManager.CurrentLanguage = value;
        }
    }
}