using System.Drawing;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Math;
using Color = GameEngine.Utility.Color;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public class GUILabel : GUIComponent {
        private TextSprite textSprite;

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

            textSprite = GameObject.AddComponent<TextSprite>(text, 16, font, color, (Width, Height));
            
            OnSizeChanged += () => {
                textSprite.Size = new Vector2(WorldWidth, WorldHeight);

                ResolveDockingCoordinates(Dock, WorldWidth, WorldHeight, out float x, out float y);
                textSprite.Offset = new Vector2(x, y);
            };
        }

        public string Text {
            get => this.textSprite.Text;
            set => this.textSprite.Text = value;
        }

        public FontFamily Font {
            get => this.textSprite.Font;
            set => this.textSprite.Font = value;
        }

        public Color TextColor {
            get => this.textSprite.Color;
            set => this.textSprite.Color = value;
        }

        public GUIDock TextDock {
            get => this.textSprite.Dock;
            set => this.textSprite.Dock = value;
        }
    }
}