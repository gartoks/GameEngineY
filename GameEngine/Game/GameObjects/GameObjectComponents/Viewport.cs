using GameEngine.Math;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class Viewport : GOC {



        public void ScreenToViewport(ref Vector2 p) {
            float x = (p.x / Window.Window.Width - 0.5f) * Width;
            float y = -(p.y / Window.Window.Height - 0.5f) * Height;

            p.x = x;
            p.y = y;
        }

        public void ViewportToScreen(ref Vector2 p) {
            float x = (p.x / Width + 0.5f) * Window.Window.Width;
            float y = (-p.y / Height + 0.5f) * Window.Window.Height;

            p.x = x;
            p.y = y;
        }

        public void WorldToViewport(ref Vector2 p) {
            float sin = Mathf.Sin(Transform.GlobalRotation);
            float cos = Mathf.Cos(Transform.GlobalRotation);

            float x = cos * Transform.GlobalScale.x * (p.x - Transform.GlobalPosition.x) - sin * Transform.GlobalScale.y * (p.y - Transform.GlobalPosition.y);
            x /= Zoom;
            float y = sin * Transform.GlobalScale.x * (p.x - Transform.GlobalPosition.x) + cos * Transform.GlobalScale.y * (p.y - Transform.GlobalPosition.y);
            y /= Zoom;

            p.x = x;
            p.y = y;
        }

        public void ViewportToWorld(ref Vector2 p) {
            float sin = Mathf.Sin(Transform.GlobalRotation);
            float cos = Mathf.Cos(Transform.GlobalRotation);
            float tan = sin / cos;
            float tanSin = tan * sin;

            float y = (p.y - tan * p.x) * Zoom / Transform.GlobalScale.y + (tanSin - cos) * Transform.GlobalPosition.y;
            y /= tanSin + cos;

            float x = p.x * Zoom + sin * Transform.GlobalScale.y * (y - Transform.GlobalPosition.y);
            x /= cos * Transform.GlobalScale.x;
            x += Transform.GlobalPosition.x;

            p.x = x;
            p.y = y;
        }

        public float Zoom => ;

        public float Width =>;

        public float Height =>;

        public float WorldWidth =>;

        public float WorldHeight =>;


    }
}