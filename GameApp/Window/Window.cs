using System;
using System.Drawing;
using GameApp.Application;
using GameApp.Files;
using GameApp.Settings;
using GameEngine.Window;
using OpenTK;
using OpenTK.Graphics;
using static GameApp.Application.AppConstants.SettingKeys;
using Color = GameEngine.Utility.Color;

namespace GameApp.Window {
    internal class Window : IWindow {
        private static Window instance;
        internal static Window Instance {
            get => Window.instance;
            private set { if (Window.instance != null) throw new InvalidOperationException("Only one instance of game hook type permitted."); else instance = value; }
        }

        private ScreenMode screenMode;

        private GameWindow gameWindow;

        internal Window() {
            Instance = this;
        }

        internal void Install() {
            SettingsManager.Instance.InstallSetting(APP_FRAMES_PER_SECOND, AppConstants.Defaults.APP_FRAMES_PER_SECOND, x => x.ToString(), false, true);
            SettingsManager.Instance.InstallSetting(WINDOW_RESOLUTION_X, AppConstants.Defaults.WINDOW_RESOLUTION_X, x => x.ToString(), false, true);
            SettingsManager.Instance.InstallSetting(WINDOW_RESOLUTION_Y, AppConstants.Defaults.WINDOW_RESOLUTION_Y, x => x.ToString(), false, true);
            SettingsManager.Instance.InstallSetting(WINDOW_VSYNC, AppConstants.Defaults.WINDOW_VSYNC, x => x.ToString(), false, true);
            SettingsManager.Instance.InstallSetting(WINDOW_SCREEN_MODE, AppConstants.Defaults.WINDOW_SCREEN_MODE, x => x.ToString(), false, true);
            SettingsManager.Instance.InstallSetting(WINDOW_COLOR_FORMAT, AppConstants.Defaults.WINDOW_COLOR_FORMAT, x => x.ToString(), true, false);
            SettingsManager.Instance.InstallSetting(WINDOW_DEPTH_BUFFER_SIZE, AppConstants.Defaults.WINDOW_DEPTH_BUFFER_SIZE, x => x.ToString(), true, false);
            SettingsManager.Instance.InstallSetting(WINDOW_STENCIL_BUFFER_SIZE, AppConstants.Defaults.WINDOW_STENCIL_BUFFER_SIZE, x => x.ToString(), true, false);
        }

        internal bool VerifyInstallation() {
            return
                SettingsManager.Instance.HasSetting(APP_FRAMES_PER_SECOND) &&
                SettingsManager.Instance.HasSetting(WINDOW_RESOLUTION_X) &&
                SettingsManager.Instance.HasSetting(WINDOW_RESOLUTION_Y) &&
                SettingsManager.Instance.HasSetting(WINDOW_VSYNC) &&
                SettingsManager.Instance.HasSetting(WINDOW_SCREEN_MODE) &&
                SettingsManager.Instance.HasSetting(WINDOW_COLOR_FORMAT) &&
                SettingsManager.Instance.HasSetting(WINDOW_DEPTH_BUFFER_SIZE) &&
                SettingsManager.Instance.HasSetting(WINDOW_STENCIL_BUFFER_SIZE);
        }

        internal void Initialize(Icon icon) {
            SettingsManager.Instance.AddListener((setting, newSetting, oldSetting) => {
                switch (setting) {
                    case WINDOW_RESOLUTION_X:
                        Width = int.Parse(newSetting);
                        break;
                    case WINDOW_RESOLUTION_Y:
                        Height = int.Parse(newSetting);
                        break;
                    case WINDOW_SCREEN_MODE:
                        ScreenMode sM = (ScreenMode)Enum.Parse(typeof(ScreenMode), newSetting);
                        ScreenMode = sM;
                        break;
                    case WINDOW_VSYNC:
                        VSync = bool.Parse(newSetting);
                        break;
                    case APP_FRAMES_PER_SECOND:
                        FramesPerSecond = int.Parse(newSetting);
                        break;
                }
            });

            int resolutionX = SettingsManager.Instance.Get(WINDOW_RESOLUTION_X, int.Parse);
            int resolutionY = SettingsManager.Instance.Get(WINDOW_RESOLUTION_Y, int.Parse);

            GraphicsMode graphicsMode = new GraphicsMode(
                SettingsManager.Instance.Get(WINDOW_COLOR_FORMAT, int.Parse),
                SettingsManager.Instance.Get(WINDOW_DEPTH_BUFFER_SIZE, int.Parse),
                SettingsManager.Instance.Get(WINDOW_STENCIL_BUFFER_SIZE, int.Parse),
                0);

            this.gameWindow = new GameWindow(resolutionX, resolutionY, graphicsMode);

            ScreenMode = SettingsManager.Instance.Get(WINDOW_SCREEN_MODE, s => (ScreenMode)Enum.Parse(typeof(ScreenMode), s));
            VSync = SettingsManager.Instance.Get(WINDOW_VSYNC, bool.Parse);

            this.gameWindow.Icon = icon;
        }

        internal void Show() {
            int fps = SettingsManager.Instance.Get(APP_FRAMES_PER_SECOND, int.Parse);

            this.gameWindow.Run(1f, 1f / fps);
        }

        // svw (cosphi (px - vx) scx - sinphi (py - vy) sy) / sww
        // svh (sinphi (px - vx) scx + cosphi (py - vy) sy) / swh

        internal void Close() {
            GameWindow.Close();
        }

        internal Point ScreenToWindow(int x, int y) {
            return GameWindow.PointToClient(new Point(x, y));
        }

        internal Point WindowToScreen(int x, int y) {
            return GameWindow.PointToScreen(new Point(x, y));
        }

        internal void SwapBuffers() {
            this.GameWindow.SwapBuffers();
        }

        public void SetMouseCursor(int hotspotX, int hotspotY, string imageFile) {
            Color[,] colorData = FileManager.Instance.LoadFileAsync(() => {
                Bitmap bmp = (Bitmap)Image.FromFile(imageFile);

                Color[,] cD = new Color[bmp.Width, bmp.Height];
                for (int y = 0; y < bmp.Height; y++) {
                    for (int x = 0; x < bmp.Width; x++) {
                        cD[x, y] = new Color(bmp.GetPixel(x, y));
                    }
                }

                return cD;
            }).Result;

            SetMouseCursor(hotspotX, hotspotY, colorData);
        }

        public void SetMouseCursor(int hotspotX, int hotspotY, Color[,] imageData) {
            int width = imageData.GetLength(0);
            int height = imageData.GetLength(1);

            byte[] byteData = new byte[width * height * 4];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte alpha = imageData[x, y].A;
                    byteData[x + y * width + 0] = (byte)(imageData[x, y].B * alpha / 255);
                    byteData[x + y * width + 1] = (byte)(imageData[x, y].G * alpha / 255);
                    byteData[x + y * width + 2] = (byte)(imageData[x, y].R * alpha / 255);
                    byteData[x + y * width + 3] = alpha;
                }
            }

            this.gameWindow.Cursor = new MouseCursor(hotspotX, hotspotY, width, height, byteData);
        }

        public bool IsMouseCursorVisible {
            get => this.gameWindow.CursorVisible;
            set => this.gameWindow.CursorVisible = value;
        }

        public int FramesPerSecond {
            get => (int)this.gameWindow.RenderFrequency;
            internal set => this.gameWindow.TargetRenderFrequency = value;
        }

        public string Title {
            get => this.gameWindow.Title; set => this.gameWindow.Title = value;
        }

        public int Width {
            get => this.gameWindow.Width;
            internal set => this.gameWindow.Width = value;
        }

        public int Height {
            get => this.gameWindow.Height;
            set => this.gameWindow.Height = value;
        }

        public ScreenMode ScreenMode {
            get => this.screenMode;
            internal set {
                this.screenMode = value;

                if (value == ScreenMode.Fullscreen) {
                    this.gameWindow.ClientSize = new Size(DisplayDevice.Default.Width, DisplayDevice.Default.Height);
                    this.gameWindow.WindowState = WindowState.Fullscreen;
                    this.gameWindow.WindowBorder = WindowBorder.Fixed;
                } else if (value == ScreenMode.Windowed) {
                    this.gameWindow.Width = SettingsManager.Instance.Get(WINDOW_RESOLUTION_X, int.Parse);
                    this.gameWindow.Height = SettingsManager.Instance.Get(WINDOW_RESOLUTION_Y, int.Parse);
                    this.gameWindow.WindowState = WindowState.Normal;
                    this.gameWindow.WindowBorder = WindowBorder.Fixed;
                } else if (value == ScreenMode.BorderlessWindow) {
                    this.gameWindow.ClientSize = new Size(DisplayDevice.Default.Width, DisplayDevice.Default.Height);
                    this.gameWindow.WindowBorder = WindowBorder.Hidden;
                }
            }
        }

        public bool VSync {
            get => GameWindow.VSync == VSyncMode.On;
            internal set => GameWindow.VSync = value ? VSyncMode.On : VSyncMode.Off;
        }

        public Icon Icon {
            get => this.gameWindow.Icon; set => this.gameWindow.Icon = value;
        }

        internal GameWindow GameWindow => this.gameWindow;

    }
}