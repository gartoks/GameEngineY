using System;
using System.Drawing;
using System.Threading;
using GameEngine.Game;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents;
using GameEngine.Game.GameObjects.Prefabs;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Modding;
using GameEngine.Resources;
using GameEngine.Resources.Loaders;
using GameEngine.Window;
using TestGame.CellularAutomata;
using TestGame.CellularAutomata.ForestFire;
using Color = GameEngine.Utility.Color;
using Rectangle = GameEngine.Math.Shapes.Rectangle;

namespace TestGame {
    public class Mod : ModBase {
        public override void Initialize() {
            Log.WriteLine($"Initialize");

            Scene.LoadScene("MainScene");
        }

        public override void OnSceneLoad(string sceneName) {
            Log.WriteLine($"OnSceneLoad {sceneName}");

            OnSceneLoad_CellularAutomata();
        }

        private void OnSceneLoad_CellularAutomata() {
            IGameObject go_viewport = Scene.CreateGameObject("go_viewport");
            Viewport viewport = go_viewport.AddComponent<Viewport>();
            Scene.MainViewport = viewport;
            GUIHandler guiHandler = go_viewport.AddComponent<GUIHandler>();

            IGameObject gO_cellularAutomata = Scene.CreateGameObject("gO_cellularAutomata");
            ForestFire forestFire = gO_cellularAutomata.AddComponent<ForestFire>(new CellularAutomataInitializationData(2, 100, true, NeighbourhoodMode.Moore));
            gO_cellularAutomata.Transform.Position.x -= (viewport.Width - 1) / 2f;

            IGameObject gO_pieChart = Scene.CreateGameObject("go_pieChart");
            PieChart pieChart = gO_pieChart.AddComponent<PieChart>(forestFire);
            gO_pieChart.Transform.Scale = new Vector2(0.25f, 0.25f);
            gO_pieChart.Transform.Position.x += 0.25f;
        }

        private void OnSceneLoad_Test() {
            //ResourceManager.LoadResource<string, TextLoadingParameters>("res_str_test_0", new TextLoadingParameters(new[] { "res_str_test_0.txt" }, Encoding.UTF8), 1, false);
            //if (!ResourceManager.TryGetResource("res_str_test_0", out string str, true))
            //    Log.WriteLine("Could not get resource 'res_str_test_0'", LogType.Message);

            ResourceManager.LoadResource<ITexture, TextureLoadingParameters>("res_tex_test_0", new TextureLoadingParameters(new[] { "res_tex_test_0.png" }, TextureWrapMode.ClampToEdge, TextureFilterMode.Nearest), 2, false);
            ResourceManager.LoadResource<ITexture, TextureLoadingParameters>("res_tex_test_1", new TextureLoadingParameters(new[] { "res_tex_test_1.png" }, TextureWrapMode.ClampToEdge, TextureFilterMode.Nearest), 2, false);
            ResourceManager.LoadResource<ITexture, TextureLoadingParameters>("res_tex_test_2", new TextureLoadingParameters(new[] { "res_tex_micha.png" }, TextureWrapMode.ClampToEdge, TextureFilterMode.Nearest), 2, false);
            ResourceManager.LoadResource<ITextureAtlas, FontLoadingParameters>($"font_Consolas_16", new FontLoadingParameters(new FontFamily("Consolas"), 16), 0, true);

            if (!ResourceManager.TryGetResource("res_tex_test_0", out ITexture tex0, true))
                Log.WriteLine("Could not get resource 'res_tex_test_0'", LogType.Message);

            if (!ResourceManager.TryGetResource("res_tex_test_1", out ITexture tex1, true))
                Log.WriteLine("Could not get resource 'res_tex_test_1'", LogType.Message);

            if (!ResourceManager.TryGetResource("res_tex_test_2", out ITexture tex2, true))
                Log.WriteLine("Could not get resource 'res_tex_test_2'", LogType.Message);

            IGameObject go_viewport = Scene.CreateGameObject("go_viewport");
            Viewport viewport = go_viewport.AddComponent<Viewport>();
            //viewport.Transform.Position = new Vector2(1, 0);
            Scene.MainViewport = viewport;
            viewport.Zoom = 1;
            //viewport.Width *= 10f;
            //viewport.Height *= 10f;
            //viewport.Transform.Position.x += 1;
            GUIHandler guiHandler = go_viewport.AddComponent<GUIHandler>();

            ResourceManager.TryGetResource("internal_circleShader", out IShader shader, true);

            //IGameObject go_sprite = Scene.CreateGameObject("go_sprite");
            //go_sprite.Transform.Z = 0.65f;
            //Sprite sprite = go_sprite.AddComponent<Sprite>();
            //sprite.Transform.Scale = new Vector2(0.95f, 0.95f);
            //sprite.Color = new Color(255, 0, 255, 63);

            IGameObject go_cSprite = Scene.CreateGameObject("go_cSprite");
            go_cSprite.Transform.Z = 0.65f;

            // uniforms
            Color cSpriteColor = Color.WHITE;
            float innerRadius = 0.1f;
            float startAngle = 0.0f;
            float sweepAngle = 1f;

            // vertex attributes
            (VertexAttribute va, float[][] data)[] vertexAttributes = {
                (new VertexAttribute("in_localCoords", VertexAttributeType.FloatVector2),
                new[] {
                    new[] {-0.5f, 0.5f},
                    new[] {0.5f, 0.5f},
                    new[] {0.5f, -0.5f},
                    new[] {-0.5f, -0.5f}
                })
            };
            Action<IShaderUniformAssigner, IUniform> uniformAssigner = new Action<IShaderUniformAssigner, IUniform>((assigner, uniform) => {
                if (uniform.Name.Equals("u_color"))
                    assigner.SetUniform(uniform.Name, cSpriteColor.r, cSpriteColor.g, cSpriteColor.b, cSpriteColor.a);
                else if (uniform.Name.Equals("u_innerRadius"))
                    assigner.SetUniform(uniform.Name, innerRadius);
                else if (uniform.Name.Equals("u_startAngle"))
                    assigner.SetUniform(uniform.Name, startAngle);
                else if (uniform.Name.Equals("u_sweepAngle"))
                    assigner.SetUniform(uniform.Name, sweepAngle);
            });

            CustomSprite cSprite = go_cSprite.AddComponent<CustomSprite>(shader, vertexAttributes, uniformAssigner);
            cSprite.Transform.Scale = new Vector2(0.95f, 0.95f);

            IGameObject go_slider_innerRadius = Scene.CreateGameObject("go_slider_innerRadius", new Vector2(Scene.MainViewport.Left, Scene.MainViewport.Bottom));
            GUISlider guiSlider_innerRadius = go_slider_innerRadius.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.DARK_RED);
            guiSlider_innerRadius.Value = innerRadius;
            Thread.Sleep(100);
            guiSlider_innerRadius.Width = 0.3f;
            guiSlider_innerRadius.Height = 0.055f;
            guiSlider_innerRadius.Dock = GUIDock.BottomLeft;
            guiSlider_innerRadius.OnValueChanged += slider => innerRadius = slider.Value;
            Thread.Sleep(100);

            IGameObject go_slider_startAngle = Scene.CreateGameObject("go_slider_startAngle", new Vector2(Scene.MainViewport.Left + guiSlider_innerRadius.WorldWidth * 1.05f, Scene.MainViewport.Bottom));
            GUISlider guiSlider_startAngle = go_slider_startAngle.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.DARK_GREEN);
            guiSlider_startAngle.Value = startAngle;
            Thread.Sleep(100);
            guiSlider_startAngle.Width = 0.3f;
            guiSlider_startAngle.Height = 0.055f;
            guiSlider_startAngle.Dock = GUIDock.BottomLeft;
            guiSlider_startAngle.OnValueChanged += slider => startAngle = slider.Value;
            Thread.Sleep(100);

            IGameObject go_slider_sweepAngle = Scene.CreateGameObject("go_slider_sweepAngle", new Vector2(Scene.MainViewport.Left + guiSlider_innerRadius.WorldWidth * 1.05f + guiSlider_startAngle.WorldWidth * 1.05f, Scene.MainViewport.Bottom));
            GUISlider guiSlider_sweepAngle = go_slider_sweepAngle.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.DARK_BLUE);
            guiSlider_sweepAngle.Value = sweepAngle;
            Thread.Sleep(100);
            guiSlider_sweepAngle.Width = 0.3f;
            guiSlider_sweepAngle.Height = 0.055f;
            guiSlider_sweepAngle.Dock = GUIDock.BottomLeft;
            guiSlider_sweepAngle.OnValueChanged += slider => sweepAngle = slider.Value;
            Thread.Sleep(100);

            IGameObject go_slider_red = Scene.CreateGameObject("go_slider_red", new Vector2(Scene.MainViewport.Left, Scene.MainViewport.Bottom + guiSlider_innerRadius.WorldHeight * 1.05f));
            GUISlider guiSlider_red = go_slider_red.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.RED);
            guiSlider_red.Value = 1;
            Thread.Sleep(100);
            guiSlider_red.Width = 0.3f;
            guiSlider_red.Height = 0.055f;
            guiSlider_red.Dock = GUIDock.BottomLeft;
            guiSlider_red.OnValueChanged += slider => cSpriteColor.r = slider.Value;
            Thread.Sleep(100);

            IGameObject go_slider_green = Scene.CreateGameObject("go_slider_green", new Vector2(Scene.MainViewport.Left + guiSlider_red.WorldWidth * 1.05f, Scene.MainViewport.Bottom + guiSlider_innerRadius.WorldHeight * 1.05f));
            GUISlider guiSlider_green = go_slider_green.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.GREEN);
            guiSlider_green.Value = 1;
            Thread.Sleep(100);
            guiSlider_green.Width = 0.3f;
            guiSlider_green.Height = 0.055f;
            guiSlider_green.Dock = GUIDock.BottomLeft;
            guiSlider_green.OnValueChanged += slider => cSpriteColor.g = slider.Value;
            Thread.Sleep(100);

            IGameObject go_slider_blue = Scene.CreateGameObject("go_slider_blue", new Vector2(Scene.MainViewport.Left + guiSlider_red.WorldWidth * 1.05f + guiSlider_green.WorldWidth * 1.05f, Scene.MainViewport.Bottom + guiSlider_innerRadius.WorldHeight * 1.05f));
            GUISlider guiSlider_blue = go_slider_blue.AddComponent<GUISlider>(Color.LIGHT_GRAY, Color.BLUE);
            guiSlider_blue.Value = 1;
            Thread.Sleep(100);
            guiSlider_blue.Width = 0.3f;
            guiSlider_blue.Height = 0.055f;
            guiSlider_blue.Dock = GUIDock.BottomLeft;
            guiSlider_blue.OnValueChanged += slider => cSpriteColor.b = slider.Value;
            Thread.Sleep(100);

            //IGameObject go_text = Scene.CreateGameObject("go_text");
            //TextSprite tSprite = go_text.AddComponent<TextSprite>();
            //tSprite.Dock = GUIDock.Centered;

            //IGameObject go_guiPanel = Scene.CreateGameObject("go_guiPanel", new Vector2(viewport.Left, viewport.Bottom));
            //go_guiPanel.Transform.Z = 0.5f;
            //GUIPanel guiPanel = go_guiPanel.AddComponent<GUIPanel>();
            //guiPanel.Dock = GUIDock.BottomLeft;
            //guiPanel.Width = 0.1f;
            //guiPanel.Height = 0.1f;
            //guiPanel.OnMouseEntered += (component, x, y) => Log.WriteLine("Entered");
            //guiPanel.OnMouseExited += (component, x, y) => Log.WriteLine("Exited");

            //IGameObject go_guiLabel = Scene.CreateGameObject("go_guiLabel");
            //go_guiLabel.Transform.Z = 0.5f;
            //GUILabel guiLabel = go_guiLabel.AddComponent<GUILabel>("Hallo Welt", new FontFamily("Consolas"), Color.RED);
            //guiLabel.Height = 0.1f;
            //guiLabel.Width = 0.4f;
            //guiLabel.TextColor = Color.RED;

            //IGameObject go_guiButton = Scene.CreateGameObject("go_guiButton", new Vector2(viewport.Left, viewport.Bottom));
            //GUIButton guiButton = go_guiButton.AddComponent<GUIButton>("Hallo Welt", new FontFamily("Consolas")/*, colors, graphics*/);
            //guiButton.Width = 0.1f;
            //guiButton.Height = 0.1f;
            //guiButton.Dock = GUIDock.BottomLeft;
            //guiButton.OnMouseReleased += (component, x, y) => Log.WriteLine("Released");
            //guiButton.OnMouseDown += (component, x, y) => Log.WriteLine("Down");
            //guiButton.OnMouseClicked += (component, x, y) => Log.WriteLine("Click");
            //guiButton.OnMouseEntered += (component, x, y) => Log.WriteLine("Entered");
            //guiButton.OnMouseExited += (component, x, y) => Log.WriteLine("Exited");

            //IGameObject go_progressbar = Scene.CreateGameObject("go_progressbar");
            //GUIProgressbar guiProgressbar = go_progressbar.AddComponent<GUIProgressbar>();
            //guiProgressbar.Width = 0.4f;
            //guiProgressbar.Height = 0.1f;

            //DebugGUIHandler debugDisplay = DebugGUIPrefab.Create("DEBUG_DISPLAY");

            //IGameObject go_textbox = Scene.CreateGameObject("go_textbox");
            //go_textbox.Transform.Position = new Vector2(0.75f, 0);
            //IGameObject go_textbox = Scene.CreateGameObject("go_textbox", new Vector2(Scene.MainViewport.Left / 2f, Scene.MainViewport.Bottom / 2f));
            //GUITextbox guiTextbox = go_textbox.AddComponent<GUITextbox>();
            //guiTextbox.Width = 0.4f;
            //guiTextbox.Height = 0.1f;
            //guiTextbox.Dock = GUIDock.BottomLeft;

            //IGameObject go_textSprite = Scene.CreateGameObject("go_textSprite");
            //TextSprite textSprite = go_textSprite.AddComponent<TextSprite>("Hallo Welt", 64, new FontFamily("Consolas"), Color.LIME);

            //IGameObject go_test = Scene.CreateGameObject("go_test");
            //go_test.AddComponent<TextSprite>("Hello World", 32, new FontFamily("Consolas"), Color.RED, (0.4f * 0.9f, 0.1f * 0.9f));

            //ForestFire ca2 = go_gridGraphics2.AddComponent<ForestFire>(new CellularAutomataInitializationData(2, 100, true, NeighbourhoodMode.Moore));

            //IGameObject go_timer = Scene.CreateGameObject("go_timer");
            //Timer timer = go_timer.AddComponent<Timer>();
            //timer.Time = 0.01f;
            //timer.OnTimerComplete += t => {
            //    guiProgressbar.Value += 0.005f;
            //    timer.Start();
            //};
            //timer.Start();

            //Sprite sprite = gameObject.AddComponent<Sprite>();
            //sprite.Texture = tex;

        }

        public override string Name => "TestGame";

        public override string ModID => "modID_testGame";

        public override int ModLoadingPriority => 1;

        public override string SettingsFile => null;

        public override string LocalizationDirectory => null;
    }
}