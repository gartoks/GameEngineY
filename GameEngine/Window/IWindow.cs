using GameEngine.Utility;

namespace GameEngine.Window {
    public interface IWindow {

        string Title { get; set; }

        System.Drawing.Icon Icon { get; set; }

        void SetMouseCursor(int hotspotX, int hotspotY, string imageFile);

        void SetMouseCursor(int hotspotX, int hotspotY, Color[,] imageData);

        bool IsMouseCursorVisible { get; set; }

        int FramesPerSecond { get; }

        ScreenMode ScreenMode { get; }

        bool VSync { get; }

        int Width { get; }

        int Height { get; }
    }
}