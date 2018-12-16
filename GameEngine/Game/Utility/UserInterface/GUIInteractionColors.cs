using GameEngine.Utility;

namespace GameEngine.Game.Utility.UserInterface {
    public class GUIInteractionColors {
        public readonly Color Default;
        public readonly Color Hovered;
        public readonly Color Clicked;

        public GUIInteractionColors(Color @default, Color hovered, Color clicked) {
            Default = @default;
            Hovered = hovered;
            Clicked = clicked;
        }

        public Color Get(GUIComponentState state) {
            switch (state) {
                case GUIComponentState.Hovered:
                    return Hovered;
                case GUIComponentState.Clicked:
                    return Clicked;
                default:
                    return Default;
            }
        }
    }
}