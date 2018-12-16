using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameApp.Application;
using GameApp.Files;
using GameApp.Game;
using GameApp.Graphics;
using GameApp.Input;
using GameApp.Localization;
using GameApp.Resources;
using GameApp.Settings;
using GameEngine.Logging;
using GameEngine.Modding;
using Log = GameApp.Logging.Log;
using static GameApp.Application.AppConstants.Mods;

namespace GameApp.Modding {
    internal sealed class ModManager : IModManager {

        private static ModManager instance;
        internal static ModManager Instance {
            get => ModManager.instance;
            private set { if (ModManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private Tuple<ModBase, string> baseMod;
        private Dictionary<string, Tuple<ModBase, string>> installedMods;
        private readonly Dictionary<int, List<ModBase>> modPriorities;

        internal ModManager() {
            ModManager.Instance = this;

            this.installedMods = new Dictionary<string, Tuple<ModBase, string>>();
            this.modPriorities = new Dictionary<int, List<ModBase>>();
        }

        internal void Install() { }

        internal bool VerifyInstallation() => File.Exists(Path.Combine(FileManager.Instance.ModsPath, Application.Application.Instance.BaseModID, AppConstants.Files.MOD));

        internal void Initialize() {
            LoadMods();
        }

        private void LoadMods() {
            string modsDirectoryPath = FileManager.Instance.ModsPath;

            string baseModDirectory = Path.Combine(modsDirectoryPath, Application.Application.Instance.BaseModID);
            LoadMod(baseModDirectory, true);

            foreach (string modDirectory in Directory.EnumerateDirectories(modsDirectoryPath)) {
                if (modDirectory.Equals(baseModDirectory))
                    continue;

                LoadMod(modDirectory, false);
            }

            this.installedMods = this.installedMods.OrderBy(kv => kv.Key).ToDictionary(item => item.Key, item => item.Value);
        }

        internal void InitializeMods() {
            BaseMod.Initialize();

            foreach (ModBase mod in Mods) {
                mod.Initialize();
            }
        }

        private void LoadMod(string modDirectory, bool isBaseMod) {
            try {
                string assemblyFile = Path.Combine(modDirectory, AppConstants.Files.MOD);
                if (!File.Exists(assemblyFile)) {
                    Log.Instance.WriteLine($"Mod file in '{modDirectory}' does not exist.", LogType.Error);
                    return;
                }

                Assembly assembly;
                try {
                    assembly = Assembly.LoadFrom(assemblyFile);
                } catch (Exception) {
                    Log.Instance.WriteLine($"Could not load mod file '{assemblyFile}'.", LogType.Error);
                    return;
                }

                Type type = assembly.GetType(Application.Application.Instance.Name + "." + MOD_ENTRY_CLASS_NAME);

                if (type == null) {
                    Log.Instance.WriteLine($"Could not find '{MOD_ENTRY_CLASS_NAME}' class in default namespace for '{assemblyFile}'.", LogType.Error);
                    return;
                }

                ModBase mod = (ModBase)Activator.CreateInstance(type);

                // not the base mod
                if (mod.ModID.Length < MOD_ID_MIN_LENGTH || char.ToLower(mod.ModID[0]) < 'a' || char.ToLower(mod.ModID[0]) > 'z') {
                    Log.Instance.WriteLine($"Invalid mod id: '{mod.ModID}'. Must be at least {MOD_ID_MIN_LENGTH} characters long and begin with a letter (a-z, A-Z).", LogType.Error);
                    return;
                }

                if (isBaseMod) {
                    // base mod
                    this.baseMod = new Tuple<ModBase, string>(mod, modDirectory);
                } else {
                    if (this.installedMods.ContainsKey(mod.ModID)) {
                        Log.Instance.WriteLine($"Mod with id '{mod.ModID}' already exists.", LogType.Error);
                        return;
                    }

                    if (mod.ModLoadingPriority <= 0) {
                        Log.Instance.WriteLine($"Mod with id '{mod.ModID}' has an invalid loading priority.", LogType.Error);
                        return;
                    }

                    this.installedMods[mod.ModID] = new Tuple<ModBase, string>(mod, modDirectory);

                    if (!this.modPriorities.ContainsKey(mod.ModLoadingPriority))
                        this.modPriorities[mod.ModLoadingPriority] = new List<ModBase>();

                    this.modPriorities[mod.ModLoadingPriority].Add(mod);
                }

                LocalizationManager.Instance.ReloadLanguage();
                SettingsManager.Instance.LoadModSettings(mod);


                mod.OnLoad(isBaseMod,
                    Application.Application.Instance,
                    Log.Instance,
                    FileManager.Instance,
                    SettingsManager.Instance,
                    ResourceManager.Instance,
                    LocalizationManager.Instance,
                    Window.Window.Instance,
                    GraphicsHandler.Instance,
                    InputManager.Instance,
                    TimeManager.Instance,
                    SceneManager.Instance,
                    ModManager.instance);
            } catch (Exception e) {
                Log.Instance.WriteLine($"Could not load mod '{modDirectory}'.", LogType.Error);
                Log.Instance.WriteLine(e.ToString(), LogType.Error);
                Log.Instance.WriteLine(e.StackTrace, LogType.Error);
            }
        }

        public bool IsModLoaded(string modID) {
            return this.installedMods.ContainsKey(modID);
        }

        internal ModBase GetMod(string modID) {
            if (!this.installedMods.ContainsKey(modID)) {
                return null;
            }

            return this.installedMods[modID].Item1;
        }

        internal ModBase BaseMod => this.baseMod.Item1;

        internal IEnumerable<ModBase> Mods {
            get { return this.installedMods.Select(m => m.Value.Item1); }
        }

        public IEnumerable<string> InstalledMods => this.installedMods.Keys;

        internal bool GetModDirectory(string modID, out string modDirectory) {
            if (modID.Equals(BaseMod.ModID)) {
                modDirectory = this.baseMod.Item2;
                return true;
            }

            if (!this.installedMods.ContainsKey(modID)) {
                modDirectory = null;
                return false;
            }

            modDirectory = this.installedMods[modID].Item2;

            return true;
        }

        public IEnumerable<(string key, object data)> GetCustomModData(string modID) {
            if (modID == null || modID.Length < MOD_ID_MIN_LENGTH) {
                Log.Instance.WriteLine($"Mod with id '{modID}' does not exist.", LogType.Error);
                return new (string, object)[0];
            }

            ModBase mod = GetMod(modID);
            return mod.CustomModData ?? new (string, object)[0];
        }
    }
}