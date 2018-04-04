namespace GameEngine.Localization {
    /// <summary>
    /// Interface to localize text.
    /// </summary>
    public interface ILocalizationManager {
        /// <summary>
        /// Localizes the specified key to the currently loaded language see <see cref="CurrentLanguage"/>. If the key could not be found the key itself is returned and a warning is logged.
        /// </summary>
        /// <param name="localizationKey">The localization key.</param>
        /// <returns>Returns the associated, localized text to the key. </returns>
        string Localize(string localizationKey);

        /// <summary>
        /// Returns the currently loaded language.
        /// </summary>
        string CurrentLanguage { get; set; }
    }
}