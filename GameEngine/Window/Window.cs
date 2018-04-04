using GameEngine.Modding;
using GameEngine.Utility;

namespace GameEngine.Window {
    public static class Window {

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public static string Title {
            get => ModBase.Window.Title;
            set => ModBase.Window.Title = value;
        }

        /// <summary>
        /// Gets or sets the window icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public static System.Drawing.Icon Icon {
            get => ModBase.Window.Icon;
            set => ModBase.Window.Icon = value;
        }

        /// <summary>
        /// Sets the mouse cursor graphic and the hotspot relative to image coordinates at which a mouse click will be detected.
        /// </summary>
        /// <param name="hotspotX">The hotspot x.</param>
        /// <param name="hotspotY">The hotspot y.</param>
        /// <param name="imageFile">The image file.</param>
        public static void SetMouseCursor(int hotspotX, int hotspotY, string imageFile) {
            ModBase.Window.SetMouseCursor(hotspotX, hotspotY, imageFile);
        }

        /// <summary>
        /// Sets the mouse cursor graphic and the hotspot relative to image coordinates at which a mouse click will be detected.
        /// </summary>
        /// <param name="hotspotX">The hotspot x.</param>
        /// <param name="hotspotY">The hotspot y.</param>
        /// <param name="imageData">The image data.</param>
        public static void SetMouseCursor(int hotspotX, int hotspotY, Color[,] imageData) {
            ModBase.Window.SetMouseCursor(hotspotX, hotspotY, imageData);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse cursor is visible or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the mouse cursor is visible; otherwise, <c>false</c>.
        /// </value>
        public static bool MouseCursorVisible {
            get => ModBase.Window.IsMouseCursorVisible;
            set => ModBase.Window.IsMouseCursorVisible = value;
        }

        /// <summary>
        /// Gets the frames per second.
        /// </summary>
        /// <value>
        /// The frames per second.
        /// </value>
        public static int FramesPerSecond => ModBase.Window.FramesPerSecond;

        /// <summary>
        /// Gets the screen mode.
        /// </summary>
        /// <value>
        /// The screen mode.
        /// </value>
        public static ScreenMode ScreenMode => ModBase.Window.ScreenMode;

        /// <summary>
        /// Gets a value indicating whether vsync is on or off.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vsync is on; otherwise, <c>false</c>.
        /// </value>
        public static bool VSync => ModBase.Window.VSync;

        /// <summary>
        /// Gets the window width in pixels.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public static int Width => ModBase.Window.Width;

        /// <summary>
        /// Gets the window height in pixels.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public static int Height => ModBase.Window.Height;
    }
}