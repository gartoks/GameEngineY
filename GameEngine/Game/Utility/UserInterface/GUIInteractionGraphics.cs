using GameEngine.Graphics;

namespace GameEngine.Game.Utility.UserInterface {
    public class GUIInteractionGraphics {
        public readonly ITexture Default;
        public readonly ITexture Hovered;
        public readonly ITexture Clicked;

        public GUIInteractionGraphics(ITexture @default, ITexture hovered, ITexture clicked) {
            Default = @default;
            Hovered = hovered;
            Clicked = clicked;
        }

        public ITexture Get(GUIComponentState state) {
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