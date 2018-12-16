using System;
using System.Linq;
using Microsoft.Win32;
using Version = GameEngine.Utility.Version;

namespace GameApp.Utility {
    internal static class RegistryHandler {

        internal static void Install(string appName, Version version, bool reinstall = false) {
            CheckParameterValidity(appName, version);

            if (reinstall)
                Uninstall(appName, version);

            if (!IsInstalled(appName))
                MainKey.CreateSubKey(appName + "\\" + "Version_" + version.ToVersionString());

            //if (!IsInstalled(appName, version)) {
            //    RegistryKey registryKey = AppKey(appName);
            //    AppKey(appName).CreateSubKey("Version_" + version.ToVersionString()); // TODO!!!!
            //}
        }

        internal static void Uninstall(string appName) {
            CheckParameterValidity(appName);

            if (!IsInstalled(appName))
                return;

            MainKey.DeleteSubKeyTree(appName);
        }

        internal static void Uninstall(string appName, Version version) {
            CheckParameterValidity(appName, version);

            if (!IsInstalled(appName, version))
                return;

            AppKey(appName).DeleteSubKeyTree("Version_" + version.ToVersionString());
        }

        internal static void SetValue(string appName, Version version, string key, object value) {
            CheckParameterValidity(appName, version);

            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;

            VersionKey(appName, version).SetValue(key, value);
        }

        internal static void DeleteValue(string appName, Version version, string key) {
            CheckParameterValidity(appName, version);

            if (string.IsNullOrWhiteSpace(key))
                return;

            VersionKey(appName, version).DeleteValue(key);
        }

        internal static object GetValue(string appName, Version version, string key) {
            CheckParameterValidity(appName, version);

            if (string.IsNullOrWhiteSpace(key))
                return false;

            return VersionKey(appName, version).GetValue(key);
        }

        internal static bool TryGetValue(string appName, Version version, string key, out object value) {
            value = GetValue(appName, version, key);
            return value != null;
        }

        internal static bool IsInstalled(string appName) {
            CheckParameterValidity(appName);

            return MainKey.GetSubKeyNames().Contains(appName);
        }

        internal static bool IsInstalled(string appName, Version version) {
            CheckParameterValidity(appName, version);

            return IsInstalled(appName) && AppKey(appName).GetSubKeyNames().Contains("Version_" + version.ToVersionString());
        }

        private static RegistryKey MainKey => Registry.CurrentUser.OpenSubKey("Software", true);

        private static RegistryKey AppKey(string appName) {
            CheckParameterValidity(appName);

            return MainKey.OpenSubKey(appName);
        }

        private static RegistryKey VersionKey(string appName, Version version) {
            CheckParameterValidity(appName, version);

            return AppKey(appName).OpenSubKey("Version_" + version.ToVersionString());
        }

        private static void CheckParameterValidity(string appName) {
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException();
        }

        private static void CheckParameterValidity(string appName, Version version) {
            if (string.IsNullOrWhiteSpace(appName) || version == null)
                throw new ArgumentNullException();
        }
    }
}