using System.Collections.Generic;

namespace GameEngine.Modding {
    public static class ModManager {
        /// <summary>
        /// Determines whether a mod of the given id is loaded.
        /// </summary>
        /// <param name="modID">The mod id.</param>
        /// <returns>
        ///   <c>true</c> if the mod is loaded; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsModLoaded(string modID) => ModBase.ModManager.IsModLoaded(modID);

        /// <summary>
        /// Gets the mod ids of installed mods.
        /// </summary>
        /// <value>
        /// The installed mod's ids.
        /// </value>
        public static IEnumerable<string> InstalledMods => ModBase.ModManager.InstalledMods;

        /// <summary>
        /// Gets custom data provided by other mods. The result contains the modID, the key of the data and the data itself.
        /// </summary>
        /// <param name="modIDs">The mod ids for which to get the custom data. If left empty, the data from all mods is returned.</param>
        /// <returns>Returns a collection of custom mod data identified by the mod's id and further by the a key.</returns>
        public static IEnumerable<(string key, object data)> GetCustomModData(string modID) => ModBase.ModManager.GetCustomModData(modID);
    }
}