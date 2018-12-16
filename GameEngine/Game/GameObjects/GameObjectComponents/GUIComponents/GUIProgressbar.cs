using System;
using GameEngine.Graphics;
using GameEngine.Math;
using GameEngine.Math.Shapes;
using GameEngine.Utility;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    /// <summary>
    /// Initialization Parameters:
    /// <para />[0]: <seealso cref="Color" /> - Background color
    /// <para />[1]: <seealso cref="ITexture" /> - Background texture
    /// <para />[2]: <seealso cref="Color" /> - Bar color
    /// <para />[3]: <seealso cref="ITexture" /> - Bar texture
    /// <para />[4]: <seealso cref="IShader" /> - Shader
    /// </summary>
    /// <seealso cref="GUIComponent" />
    public class GUIProgressbar : GUIComponent {

        private Sprite backgroundSprite;
        private Sprite barSprite;

        private float value;

        public event Action<GUIProgressbar> OnValueChanged;

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(params object[] parameters) {
            base.Initialize();

            int pIdx = 1;

            Color bgColor = Color.LIGHT_GRAY;
            if (parameters.Length >= 1 && parameters[0] is Color bgc)
                bgColor = bgc;

            Color pbColor = Color.LIME;
            if (parameters.Length >= 2 && parameters[1] is Color pbc)
                pbColor = pbc;

            ITexture bgtex;
            if (parameters.Length >= 3 && parameters[2] is ITexture bgt)
                bgtex = bgt;
            else
                bgtex = GraphicsHandler.CreateDefaultTexture(1, 1, Color.WHITE);

            ITexture pbtex;
            if (parameters.Length >= 4 && parameters[3] is ITexture pbt)
                pbtex = pbt;
            else
                pbtex = GraphicsHandler.CreateDefaultTexture(1, 1, Color.WHITE);

            IShader sh;
            if (parameters.Length >= 5 && parameters[4] is IShader s)
                sh = s;
            else
                sh = GraphicsHandler.CreateDefaultShader(1);

            backgroundSprite = GameObject.AddComponent<Sprite>(bgColor, bgtex, sh);
            barSprite = GameObject.AddComponent<Sprite>(pbColor, pbtex, sh);

            Value = 0.0f;

            UpdateMesh();

            OnSizeChanged += UpdateMesh;
        }

        public float Value {
            get => this.value;
            set {
                this.value = Mathf.Clamp01(value);
                UpdateBarMesh();
                OnValueChanged?.Invoke(this);
            }
        }

        public Color BackgroundColor {
            get => backgroundSprite.Color;
            set => backgroundSprite.Color = value;
        }

        public Color BarColor {
            get => barSprite.Color;
            set => barSprite.Color = value;
        }

        private void UpdateMesh() {
            Rectangle wR = WorldBounds;
            wR.Translate(-Transform.Position.x, -Transform.Position.y);

            float minX = wR.Left;
            float maxX = wR.Right;
            float minY = wR.Bottom;
            float maxY = wR.Top;

            backgroundSprite.Mesh.GetVertexData(0).SetAttributeData("in_position", minX, maxY, -0.01f);
            backgroundSprite.Mesh.GetVertexData(1).SetAttributeData("in_position", maxX, maxY, -0.01f);
            backgroundSprite.Mesh.GetVertexData(2).SetAttributeData("in_position", maxX, minY, -0.01f);
            backgroundSprite.Mesh.GetVertexData(3).SetAttributeData("in_position", minX, minY, -0.01f);

            UpdateBarMesh();
        }

        private void UpdateBarMesh() {
            Rectangle wR = WorldBounds;
            wR.Translate(-Transform.Position.x, -Transform.Position.y);

            float minX = wR.Left;
            float maxX = wR.Right;
            float xW = maxX - minX;
            float minY = wR.Bottom;
            float maxY = wR.Top;

            barSprite.Mesh.GetVertexData(0).SetAttributeData("in_position", minX, maxY, 0);
            barSprite.Mesh.GetVertexData(1).SetAttributeData("in_position", minX + xW * Value, maxY, 0);
            barSprite.Mesh.GetVertexData(2).SetAttributeData("in_position", minX + xW * Value, minY, 0);
            barSprite.Mesh.GetVertexData(3).SetAttributeData("in_position", minX, minY, 0);
        }

    }
}