using System;
using System.Threading.Tasks;
using GameEngine.Modding;

namespace GameEngine.Files {
    public static class File {

        /// <summary>
        /// Loads the file asynchronous. With a custom loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="customLoader">The custom loader.</param>
        /// <returns></returns>
        public static Task<T> LoadFileAsync<T>(Func<T> customLoader) {
            return ModBase.FileManager.LoadFileAsync(customLoader);
        }

        /// <summary>
        /// Loads the file asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser used to parse from the file's text data to T.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static Task<T> LoadFileAsync<T>(Func<string, T> parser, string filePath) {
            return ModBase.FileManager.LoadFileAsync(parser, filePath);
        }

        /// <summary>
        /// Loads the file asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser used to parse from the file's lines to T.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static Task<T> LoadFileAsync<T>(Func<string[], T> parser, string filePath) {
            return ModBase.FileManager.LoadFileAsync(parser, filePath);
        }

        /// <summary>
        /// Loads the file asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser used to parse from the file's byte data to T.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static Task<T> LoadFileAsync<T>(Func<byte[], T> parser, string filePath) {
            return ModBase.FileManager.LoadFileAsync(parser, filePath);
        }

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        public static void SaveFileAsync<T>(Func<T, string> parser, string filePath, T data) {
            ModBase.FileManager.SaveFileAsync(parser, filePath, data);
        }

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        public static void SaveFileAsync<T>(Func<T, string[]> parser, string filePath, T data) {
            ModBase.FileManager.SaveFileAsync(parser, filePath, data);
        }

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        public static void SaveFileAsync<T>(Func<T, byte[]> parser, string filePath, T data) {
            ModBase.FileManager.SaveFileAsync(parser, filePath, data);
        }

        ///// <summary>
        ///// Gets the current user's data path.
        ///// </summary>
        ///// <value>
        ///// The user's data path.
        ///// </value>
        //public static string DataPath => ModBase.FileManager.DataPath;

        /// <summary>
        /// Gets the mod's directory. Should only be used to get the own directory.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns></returns>
        public static string ModDirectory(ModBase mod) => ModBase.FileManager.ModDirectory(mod);
    }
}