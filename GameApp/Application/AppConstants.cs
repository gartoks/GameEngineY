using GameEngine.Utility;
using GameEngine.Window;

namespace GameApp.Application {
    internal static class AppConstants {

        public const string APP_DATA_PATH = @"..\..\..\..\";   // TODO

        internal static class Mods {
            public const int MOD_ID_MIN_LENGTH = 3;
            public const string MOD_ENTRY_CLASS_NAME = "Mod";
            public const string MOD_FILE_NAME = "mod";
        }

        internal static class FileExtensions {
            public const string SETTINGS_FILE = "pref";
            public const string LOCALIZATION_FILE = "lang";
            public const string DLL_FILE = "dll";
        }

        internal static class Files {
            public const string SETTINGS = "settings." + FileExtensions.SETTINGS_FILE;
            public const string MOD = Mods.MOD_FILE_NAME + "." + FileExtensions.DLL_FILE;
        }

        internal static class Directories {
            public const string DATA = "data";
            public const string MODS = "mods";
        }

        internal static class Defaults {
            public static readonly Color LOG_DEFAULT_COLOR_MESSAGE = Color.WHITE;
            public static readonly Color LOG_DEFAULT_COLOR_WARNING = Color.YELLOW;
            public static readonly Color LOG_DEFAULT_COLOR_ERROR = Color.RED;
            public static readonly Color LOG_DEFAULT_COLOR_DEBUG = Color.LIME;
            public const string APP_LANGUAGE = "en-US";
            public const int APP_FRAMES_PER_SECOND = 60;
            public const int APP_UPDATES_PER_SECOND = 60;
            public const int WINDOW_RESOLUTION_X = 1280;
            public const int WINDOW_RESOLUTION_Y = 720;
            public const bool WINDOW_VSYNC = false;
            public const ScreenMode WINDOW_SCREEN_MODE = ScreenMode.Windowed;
            public const int WINDOW_COLOR_FORMAT = 32;
            public const int WINDOW_DEPTH_BUFFER_SIZE = 8;
            public const int WINDOW_STENCIL_BUFFER_SIZE = 8;
            public const string SCENE_DEFAULT_SCENE_NAME = "DefaultScene";
        }

        internal static class SettingKeys {
            public const string APP_LANGUAGE = "App_Language";
            public const string APP_UPDATES_PER_SECOND = "App_UpdatesPerSecond";
            public const string APP_FRAMES_PER_SECOND = "App_FramesPerSecond";
            public const string WINDOW_RESOLUTION_X = "Window_ResolutionX";
            public const string WINDOW_RESOLUTION_Y = "Window_ResolutionY";
            public const string WINDOW_VSYNC = "Window_VSync";
            public const string WINDOW_SCREEN_MODE = "Window_ScreenMode";
            public const string WINDOW_COLOR_FORMAT = "Window_ColorFormat";
            public const string WINDOW_DEPTH_BUFFER_SIZE = "Window_DepthBufferSize";
            public const string WINDOW_STENCIL_BUFFER_SIZE = "Window_StencilBufferSize";
        }

        internal static class Internals {
            public const string SETTINGS_XML_ROOT = "Settings";
            public const int RESOURCE_THREAD_IDLE_SLEEP_TIME = 250;
            public const uint SCENE_QUADTREE_SPLIT_MARGIN = 500;
            public const uint SCENE_QUADTREE_MERGE_MARGIN = 200;
            public static readonly Color TEXTURE_DEFAULT_COLOR = new Color(255, 0, 255);
        }
    }
}