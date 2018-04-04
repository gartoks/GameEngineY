using System.Collections.Generic;

namespace GameEngine.Modding {
    /// <summary>
    /// Interface to manage mods.
    /// </summary>
    public interface IModManager {
        /// <summary>
        /// Determines whether a mod of the given id is loaded.
        /// </summary>
        /// <param name="modID">The mod id.</param>
        /// <returns>
        ///   <c>true</c> if the mod is loaded; otherwise, <c>false</c>.
        /// </returns>
        bool IsModLoaded(string modID);

        /// <summary>
        /// Gets the mod ids of installed mods.
        /// </summary>
        /// <value>
        /// The installed mod's ids.
        /// </value>
        IEnumerable<string> InstalledMods { get; }

        /// <summary>
        /// Gets custom data provided by other mods. The result contains the modID, the key of the data and the data itself.
        /// </summary>
        /// <param name="modID">The mod id for which to get the custom data. If left empty, the data from all mods is returned.</param>
        /// <returns>Returns a collection of custom mod data identified by the mod's id and further by the a key.</returns>
        IEnumerable<(string key, object data)> GetCustomModData(string modID);
    }
}