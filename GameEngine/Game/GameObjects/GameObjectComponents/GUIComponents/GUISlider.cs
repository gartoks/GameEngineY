using GameEngine.Logging;
using GameEngine.Math.Shapes;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public class GUISlider : GUIProgressbar {

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(params object[] parameters) {
            base.Initialize(parameters);

            OnMouseClicked += OnOnMouseDown;
            OnMouseDown += OnOnMouseDown;
            OnMouseReleased += OnOnMouseDown;
        }

        private void OnOnMouseDown(GUIComponent c, float x, float y) {
            Rectangle wB = WorldBounds;
            wB.Translate(-Transform.Position.x, -Transform.Position.y);
            x -= wB.Left;

            x /= wB.Width;

            Value = x;
        }
    }
}