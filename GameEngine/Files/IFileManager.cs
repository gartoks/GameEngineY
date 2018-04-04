using System;
using System.Threading.Tasks;
using GameEngine.Modding;

namespace GameEngine.Files {
    /// <summary>
    /// Interface to access file operations inside the game.
    /// </summary>
    public interface IFileManager {

        /// <summary>
        /// Loads the file asynchronous. With a custom loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="customLoader">The custom loader.</param>
        /// <returns></returns>
        Task<T> LoadFileAsync<T>(Func<T> customLoader);

        /// <summary>
        /// Loads the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        Task<T> LoadFileAsync<T>(Func<string, T> parser, string filePath);

        /// <summary>
        /// Loads the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        Task<T> LoadFileAsync<T>(Func<string[], T> parser, string filePath);

        /// <summary>
        /// Loads the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        Task<T> LoadFileAsync<T>(Func<byte[], T> parser, string filePath);

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        void SaveFileAsync<T>(Func<T, string> parser, string filePath, T data);

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        void SaveFileAsync<T>(Func<T, string[]> parser, string filePath, T data);

        /// <summary>
        /// Saves the file asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        void SaveFileAsync<T>(Func<T, byte[]> parser, string filePath, T data);

        ///// <summary>
        ///// Gets the current user's data path.
        ///// </summary>
        ///// <value>
        ///// The user's data path.
        ///// </value>
        //string DataPath { get; }

        /// <summary>
        /// Gets the mod's directory. Should only be used to get the own directory.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns></returns>
        string ModDirectory(ModBase mod);
    }
}