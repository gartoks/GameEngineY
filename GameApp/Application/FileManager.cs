using System;
using System.IO;
using System.Threading.Tasks;
using GameApp.Application;
using GameApp.Logging;
using GameEngine.Files;
using GameEngine.Modding;
using File = System.IO.File;

namespace GameApp.Files {
    internal sealed class FileManager : IFileManager {

        private static FileManager instance;
        internal static FileManager Instance {
            get => FileManager.instance;
            private set { if (FileManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        internal FileManager() {
            Instance = this;
        }

        internal void Install() {
            InstallDirectory(AppPath);

            InstallDirectory(DataPath);

            InstallDirectory(ModsPath);

            InstallFile(SettingsFile);
        }

        internal bool VerifyInstallation() {
            if (!Directory.Exists(AppPath))
                return false;

            if (!Directory.Exists(DataPath))
                return false;

            if (!Directory.Exists(ModsPath))
                return false;

            if (!File.Exists(SettingsFile))
                return false;

            return true;
        }

        internal void Initialize() { }

        #region Load
        public async Task<T> LoadFileAsync<T>(Func<T> customLoader) {
            return customLoader();
            //return await Task.Run(customLoader);
        }

        public async Task<T> LoadFileAsync<T>(Func<string, T> parser, string filePath) {
            if (File.Exists(filePath))
                return parser(File.ReadAllText(filePath));
                //return await Task.Run(() => parser(File.ReadAllText(filePath)));

            Log.Instance.WriteLine($"File '{filePath}' does not exist.");
            return default(T);
        }

        public async Task<T> LoadFileAsync<T>(Func<string[], T> parser, string filePath) {
            if (!File.Exists(filePath)) {
                Log.Instance.WriteLine($"File '{filePath}' does not exist.");
                return default(T);
            }

            return parser(File.ReadAllLines(filePath));
            //return await Task.Run(() => parser(File.ReadAllLines(filePath)));
        }

        public async Task<T> LoadFileAsync<T>(Func<byte[], T> parser, string filePath) {
            if (!File.Exists(filePath)) {
                Log.Instance.WriteLine($"File '{filePath}' does not exist.");
                return default(T);
            }

            return parser(File.ReadAllBytes(filePath));
            //return await Task.Run(() => parser(File.ReadAllBytes(filePath)));
        }
        #endregion

        #region Save
        public async void SaveFileAsync<T>(Func<T, string> parser, string filePath, T data) {
            File.WriteAllText(filePath, parser(data));
            //await Task.Run(() => File.WriteAllText(filePath, parser(data)));
        }

        public async void SaveFileAsync<T>(Func<T, string[]> parser, string filePath, T data) {
            File.WriteAllLines(filePath, parser(data));
            //await Task.Run(() => File.WriteAllLines(filePath, parser(data)));
        }

        public async void SaveFileAsync<T>(Func<T, byte[]> parser, string filePath, T data) {
            File.WriteAllBytes(filePath, parser(data));
            //await Task.Run(() => File.WriteAllBytes(filePath, parser(data)));
        }

        #endregion

        #region Misc
        private static void InstallDirectory(string path) {
            Directory.CreateDirectory(path);
        }

        private static void InstallFile(string file) {
            File.Create(file).Close();
        }
        #endregion

        #region SpecificApplicationDirectories
        internal string ModsPath => Path.Combine(AppPath, AppConstants.Directories.MODS);
        #endregion

        #region SpecificApplicationFiles
        internal string SettingsFile => Path.Combine(DataPath, AppConstants.Files.SETTINGS);
        #endregion

        #region GeneralApplicationDirectories
        internal string ExecutablePath => AppDomain.CurrentDomain.BaseDirectory;

        internal string AppPath => ExecutablePath;

        /// <summary>
        /// Gets the current user's data path.
        /// </summary>
        /// <value>
        /// The user's data path.
        /// </value>
        public string DataPath => Path.Combine(AppPath, AppConstants.Directories.DATA);
        //public string DataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.Application.Instance.Name);
        #endregion

        /// <summary>
        /// Gets the mod's directory. Should only be used to get the own directory.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns></returns>
        public string ModDirectory(ModBase mod) {
            return ModDirectory(mod.ModID);
        }

        internal string ModDirectory(string modID) {
            if (!Modding.ModManager.Instance.GetModDirectory(modID, out string dir))
                throw new ArgumentException($"Could not find mod '{modID}'.");

            return Path.Combine(ModsPath, dir);
        }

    }
}