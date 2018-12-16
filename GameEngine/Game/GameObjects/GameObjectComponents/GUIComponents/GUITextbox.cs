using System;
using System.Drawing;
using GameEngine.Application;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.Math;
using Color = GameEngine.Utility.Color;
using Rectangle = GameEngine.Math.Shapes.Rectangle;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public class GUITextbox : GUIComponent {

        private const float CURSOR_BLINK_TIME = 0.5f;

        private Sprite sprite;
        private TextSprite textSprite;

        private string text;
        private int maxTextLength;

        private float blinkTime;

        public Action<GUITextbox> OnTextChanged;
        public Action<GUITextbox> OnTextApplied;

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(object[] parameters) {
            base.Initialize();

            string text = "";
            if (parameters.Length >= 1 && parameters[0] is string txt)
                text = txt;

            FontFamily font = new FontFamily("Consolas");
            if (parameters.Length >= 2 && parameters[1] is FontFamily fnt)
                font = fnt;

            Color color = Color.BLACK;
            if (parameters.Length >= 3 && parameters[2] is Color c)
                color = c;

            ITexture tex;
            if (parameters.Length >= 4 && parameters[3] is ITexture t)
                tex = t;
            else
                tex = GraphicsHandler.CreateDefaultTexture(1, 1, Color.WHITE);

            this.sprite = GameObject.AddComponent<Sprite>(Color.WHITE, tex);
            this.textSprite = GameObject.AddComponent<TextSprite>(text, 16, font, color, (Width, Height));
            this.textSprite.Dock = GUIDock.LeftCenter;

            this.blinkTime = -CURSOR_BLINK_TIME;

            this.text = text;
            MaxTextLength = 10;

            OnSizeChanged += UpdateMesh;

            InputHandler.AddKeyPressEventHandler(keyChar => {
                if(!HasFocus)
                    return;

                if (keyChar == '\0' || Text.Length == MaxTextLength)
                    return;

                Text += keyChar;
            });

            InputHandler.AddKeyDownEventHandler((key, modifiers) => {
                if (!HasFocus)
                    return;

                if (key == Key.Back && Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);

                if (key == Key.Enter)
                    OnTextApplied?.Invoke(this);
            });
        }

        protected override void Update() {
            if (blinkTime >= 0 && !HasFocus)
                this.textSprite.Text = Text;

            if (!HasFocus)
                return;

            float deltaTime = Time.DeltaTime;

            float prevBlinkTime = blinkTime;

            blinkTime += deltaTime;
            if (blinkTime >= CURSOR_BLINK_TIME)
                blinkTime -= 2f * CURSOR_BLINK_TIME;

            if (prevBlinkTime < 0 && blinkTime >= 0)
                this.textSprite.Text = Text + "|";
            else if (prevBlinkTime >= 0 && blinkTime < 0)
                this.textSprite.Text = Text;

            //if (InputHandler.IsKeyDown(Key.Back) && Text.Length > 0)
            //    Text = Text.Substring(0, Text.Length - 1);
        }

        private bool IsCursorVisible => HasFocus && this.blinkTime >= 0;

        public string Text {
            get => this.text;
            set {
                if (value == null)
                    return;

                this.text = value;
                this.textSprite.Text = this.text + (IsCursorVisible ? "|" : "");

                OnTextChanged?.Invoke(this);
            }
        }

        public FontFamily Font {
            get => this.textSprite.Font;
            set => this.textSprite.Font = value;
        }

        public Color TextColor {
            get => this.textSprite.Color;
            set => this.textSprite.Color = value;
        }

        public int MaxTextLength {
            get => this.maxTextLength;
            set {
                this.maxTextLength = System.Math.Max(-1, value);

                if (Text.Length > this.maxTextLength)
                    Text = Text.Substring(0, this.maxTextLength);
            }
        }

        private void UpdateMesh() {
            Rectangle wR = WorldBounds;
            wR.Translate(-Transform.Position.x, -Transform.Position.y);

            float minX = wR.Left;
            float maxX = wR.Right;
            float minY = wR.Bottom;
            float maxY = wR.Top;

            sprite.Mesh.GetVertexData(0).SetAttributeData("in_position", minX, maxY, -0.01f);
            sprite.Mesh.GetVertexData(1).SetAttributeData("in_position", maxX, maxY, -0.01f);
            sprite.Mesh.GetVertexData(2).SetAttributeData("in_position", maxX, minY, -0.01f);
            sprite.Mesh.GetVertexData(3).SetAttributeData("in_position", minX, minY, -0.01f);

            textSprite.Size = new Vector2(WorldWidth, WorldHeight);

            ResolveDockingCoordinates(Dock, WorldWidth, WorldHeight, out float x, out float y);
            textSprite.Offset = new Vector2(x, y);
        }
    }
}