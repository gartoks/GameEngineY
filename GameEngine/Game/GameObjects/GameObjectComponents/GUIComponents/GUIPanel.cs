using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Math.Shapes;
using GameEngine.Utility;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public class GUIPanel : GUIComponent {

        private Sprite sprite;

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(params object[] parameters) {
            base.Initialize();

            Color color = Color.WHITE;
            if (parameters.Length >= 1 && parameters[0] is Color c)
                color = c;

            ITexture tex;
            if (parameters.Length >= 2 && parameters[1] is ITexture t)
                tex = t;
            else
                tex = GraphicsHandler.CreateDefaultTexture(1, 1, Color.WHITE);

            IShader sh;
            if (parameters.Length >= 3 && parameters[2] is IShader s)
                sh = s;
            else
                sh = GraphicsHandler.CreateDefaultShader(1);

            sprite = GameObject.AddComponent<Sprite>(color, tex, sh);

            UpdateMesh();

            OnSizeChanged += UpdateMesh;
        }

        private void UpdateMesh() {
            Rectangle wR = WorldBounds;
            wR.Translate(-Transform.Position.x, -Transform.Position.y);

            float minX = wR.Left;
            float maxX = wR.Right;
            float minY = wR.Bottom;
            float maxY = wR.Top;

            sprite.Mesh.GetVertexData(0).SetAttributeData("in_position", minX, maxY, 0f);
            sprite.Mesh.GetVertexData(1).SetAttributeData("in_position", maxX, maxY, 0f);
            sprite.Mesh.GetVertexData(2).SetAttributeData("in_position", maxX, minY, 0f);
            sprite.Mesh.GetVertexData(3).SetAttributeData("in_position", minX, minY, 0f);
        }

    }
}