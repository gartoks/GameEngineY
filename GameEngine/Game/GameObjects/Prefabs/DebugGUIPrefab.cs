using System.Collections.Generic;
using System.Drawing;
using GameEngine.Application;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.Logging;
using GameEngine.Math;
using Color = GameEngine.Utility.Color;

namespace GameEngine.Game.GameObjects.Prefabs {
    public static class DebugGUIPrefab {
        public static DebugGUIHandler Create(string name) {
            IGameObject gO = Scene.CreateGameObject(name);
            DebugGUIHandler handler = gO.AddComponent<DebugGUIHandler>(22);

            IGameObject fpsGO = Scene.CreateGameObject("fps", gO, new Vector2(Scene.MainViewport.Right, Scene.MainViewport.Top));
            GUILabel fpsLabel = fpsGO.AddComponent<GUILabel>("fps", new FontFamily("Consolas"), Color.LIME);
            fpsLabel.Dock = GUIDock.TopRight;
            fpsLabel.Width = 0.1f;
            fpsLabel.Height = 0.06f;
            handler.FpsLabel = fpsLabel;

            IGameObject upsGO = Scene.CreateGameObject("ups", gO, new Vector2(Scene.MainViewport.Right - fpsLabel.WorldWidth * 1.1f, Scene.MainViewport.Top));
            GUILabel upsLabel = upsGO.AddComponent<GUILabel>("ups", new FontFamily("Consolas"), Color.ORANGE);
            upsLabel.Dock = GUIDock.TopRight;
            upsLabel.Width = 0.1f;
            upsLabel.Height = 0.06f;
            handler.UpsLabel = upsLabel;

            Timer fpsUpsTimer = gO.AddComponent<Timer>();
            fpsUpsTimer.Time = 0.1f;
            fpsUpsTimer.OnTimerComplete += timer => {
                fpsLabel.Text = Window.Window.FramesPerSecond + "fps";
                upsLabel.Text = Time.UpdatesPerSecond + "ups";
                timer.Start();
            };
            fpsUpsTimer.Start();
            handler.Timer = fpsUpsTimer;

            IGameObject[] logGOs = new IGameObject[handler.LineCount];
            GUILabel[] logLabels = new GUILabel[handler.LineCount];
            for (int i = 0; i < handler.LineCount; i++) {
                logGOs[i] = Scene.CreateGameObject($"log_line_{i}", gO, new Vector2(Scene.MainViewport.Left, Scene.MainViewport.Top - i * Scene.MainViewport.Height / (handler.LineCount + 1)));
                logLabels[i] = logGOs[i].AddComponent<GUILabel>($"", new FontFamily("Consolas"), Color.WHITE);
                logLabels[i].Width = 2 / 3f;
                logLabels[i].Height = 2 / (float)(handler.LineCount + 1);
            }

            for (int i = 0; i < handler.LineCount; i++) {
                logLabels[i].Dock = GUIDock.TopLeft;
                logLabels[i].TextDock = GUIDock.LeftCenter;
            }
            handler.Logs = logLabels;

            IGameObject consoleGO = Scene.CreateGameObject("console", gO, new Vector2(Scene.MainViewport.Left, Scene.MainViewport.Bottom));
            GUITextbox consoleTb = consoleGO.AddComponent<GUITextbox>("", new FontFamily("Consolas"), Color.WHITE, GraphicsHandler.CreateDefaultTexture(1, 1, new Color(31, 31, 31, 127)));
            consoleTb.Width = 2f / 3f;
            consoleTb.Height = 2f / handler.LineCount;
            consoleTb.Dock = GUIDock.BottomLeft;
            consoleTb.MaxTextLength = 40;
            handler.Console = consoleTb;

            return handler;
        }
    }

    public class DebugGUIHandler : GOC {

        public Key ShowKey = Key.F11;

        internal int LineCount { get; private set; }
        internal int FontHeight { get; private set; }

        private GUITextbox console;
        internal GUILabel[] Logs;
        internal GUILabel FpsLabel;
        internal GUILabel UpsLabel;
        internal Timer Timer;

        private List<(string input, Color color)> commandLog;

        private int previousCommandIndex;

        private bool isVisible;

        public override void Initialize(object[] parameters) {
            FontHeight = (int)parameters[0];

            LineCount = (int)(Window.Window.Height / (float)FontHeight) - 1;

            commandLog = new List<(string, Color)>();
            previousCommandIndex = 0;

            InputHandler.AddKeyUpEventHandler((key, modifiers) => {
                if (key == ShowKey) {
                    isVisible = !isVisible;

                    FpsLabel.IsEnabled = isVisible;
                    UpsLabel.IsEnabled = isVisible;
                    Timer.IsEnabled = isVisible;
                    console.IsEnabled = isVisible;
                    for (int i = 0; i < LineCount; i++) {
                        Logs[i].IsEnabled = isVisible;
                    }
                }

                if (!isVisible)
                    return;

                if (!console.HasFocus)
                    return;

                if (key != Key.Up)
                    return;

                if (commandLog.Count == 0)
                    return;

                console.Text = commandLog[commandLog.Count - 1 - previousCommandIndex].input;
                previousCommandIndex++;

                if (previousCommandIndex == commandLog.Count)
                    previousCommandIndex = 0;
            });

            Log.OnLog += (text, type, color) => AddText(text, color);
            isVisible = true;
        }

        private void OnConsoleInput(GUITextbox guiTextbox) {
            if (!isVisible)
                return;

            string inputText = guiTextbox.Text;
            guiTextbox.Text = "";

            if (string.IsNullOrWhiteSpace(inputText))
                return;

            AddText(inputText, Color.WHITE);
        }

        public void AddText(string inputText, Color color) {
            if (!isVisible)
                return;

            if (string.IsNullOrWhiteSpace(inputText))
                return;

            commandLog.Add((inputText, color));
            previousCommandIndex = 0;

            for (int i = 0; i < LineCount; i++) {
                int logIdx = commandLog.Count - 1 - (LineCount - i - 1);


                if (logIdx >= 0) {
                    Logs[i].Text = commandLog[logIdx].input;
                    Logs[i].TextColor = commandLog[logIdx].color;
                }
            }
        }

        internal GUITextbox Console {
            get => console;
            set {
                console = value;
                console.OnTextApplied += OnConsoleInput;
                console.OnFocusLost += c => {
                    if (!isVisible)
                        return;

                    previousCommandIndex = 0;
                    console.Text = "";
                };
            }
        }
    }

}