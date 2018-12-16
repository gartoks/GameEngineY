using System.Drawing;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Math;
using Color = GameEngine.Utility.Color;
using Rectangle = GameEngine.Math.Shapes.Rectangle;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public class GUIButton : GUIComponent {

        private GUIInteractionGraphics graphics;
        private GUIInteractionColors colors;

        private Sprite sprite;
        private TextSprite textSprite;

        public override void Initialize() {
            Initialize(new object[0]);
        }

        public override void Initialize(params object[] parameters) {
            base.Initialize();

            string text = "";
            if (parameters.Length >= 1 && parameters[0] is string txt)
                text = txt;

            FontFamily font = new FontFamily("Consolas");
            if (parameters.Length >= 2 && parameters[1] is FontFamily fnt)
                font = fnt;

            GUIInteractionColors colors = new GUIInteractionColors(
                Color.BLACK,
                Color.BLACK,
                Color.LIGHT_GRAY
            );
            if (parameters.Length >= 3 && parameters[2] is GUIInteractionColors c)
                colors = c;

            GUIInteractionGraphics graphics = new GUIInteractionGraphics(
                GraphicsHandler.CreateDefaultTexture(1, 1, Color.WHITE),
                GraphicsHandler.CreateDefaultTexture(1, 1, Color.LIGHT_GRAY),
                GraphicsHandler.CreateDefaultTexture(1, 1, Color.DARK_GRAY)
            );
            if (parameters.Length >= 4 && parameters[3] is GUIInteractionGraphics g)
                graphics = g;

            this.graphics = graphics;
            this.colors = colors;

            this.sprite = GameObject.AddComponent<Sprite>(Color.WHITE, graphics.Get(InteractionState));
            this.textSprite = GameObject.AddComponent<TextSprite>(text, 16, font, colors.Get(InteractionState), new Rectangle(0, 0, Width, Height));

            UpdateMesh();
            UpdateTextureAndColor();

            OnMouseEntered += (component, x, y) => UpdateTextureAndColor();
            OnMouseExited += (component, x, y) => UpdateTextureAndColor();
            OnMouseClicked += (component, x, y) => UpdateTextureAndColor();
            OnMouseReleased += (component, x, y) => UpdateTextureAndColor();

            OnSizeChanged += UpdateMesh;
        }

        public string Text {
            get => this.textSprite.Text;
            set => this.textSprite.Text = value;
        }

        public FontFamily Font {
            get => this.textSprite.Font;
            set => this.textSprite.Font = value;
        }

        private void UpdateTextureAndColor() {
            sprite.SetTexture(graphics.Get(InteractionState));
            textSprite.Color = colors.Get(InteractionState);
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