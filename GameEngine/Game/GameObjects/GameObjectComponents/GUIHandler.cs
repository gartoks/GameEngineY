using System.Collections.Generic;
using GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents;
using GameEngine.Game.Utility;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Input;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    [SingletonGOC]
    public sealed class GUIHandler : GOC {
        private HashSet<GUIComponent> hoveredComponents;

        private GUIComponent focus;

        public override void Initialize() {
            hoveredComponents = new HashSet<GUIComponent>();

            InputHandler.AddMouseButtonDownEventHandler((x, y, button) => {
                if (button != MouseButton.Left)
                    return;

                Vector2 mousePos = Scene.MainViewport.ScreenToWorld(InputHandler.MousePosition);

                IEnumerable<GUIComponent> guiComponents = Scene.FindComponents<GUIComponent>();
                foreach (GUIComponent guiComponent in guiComponents) {
                    if (!guiComponent.WorldBounds.Contains(mousePos.x, mousePos.y))
                        continue;

                    hoveredComponents.Add(guiComponent);

                    guiComponent.InteractionState = GUIComponentState.Clicked;
                    Focus = guiComponent;
                    guiComponent.InvokeMouseClicked(mousePos.x, mousePos.y);
                }
            });

            InputHandler.AddMouseButtonUpEventHandler((x, y, button) => {
                if (button != MouseButton.Left)
                    return;

                Vector2 mousePos = Scene.MainViewport.ScreenToWorld(InputHandler.MousePosition);

                IEnumerable<GUIComponent> guiComponents = Scene.FindComponents<GUIComponent>();
                foreach (GUIComponent guiComponent in guiComponents) {
                    bool hovered = guiComponent.WorldBounds.Contains(mousePos.x, mousePos.y);

                    if (!hovered && Focus == guiComponent)
                        Focus = null;

                    if (!hovered)
                        continue;

                    hoveredComponents.Add(guiComponent);

                    guiComponent.InteractionState = GUIComponentState.Hovered;
                    guiComponent.InvokeMouseReleased(mousePos.x, mousePos.y);
                }
            });

            InputHandler.AddMouseMoveEventHandler((x, y, dx, dy) => {
                Vector2 mousePos = Scene.MainViewport.ScreenToWorld(InputHandler.MousePosition);

                bool mouseDown = InputHandler.IsMouseButtonDown(MouseButton.Left);

                IEnumerable<GUIComponent> guiComponents = Scene.FindComponents<GUIComponent>();
                foreach (GUIComponent guiComponent in guiComponents) {
                    bool isHovered = guiComponent.WorldBounds.Contains(mousePos.x, mousePos.y);
                    //Logging.Log.WriteLine($"{mousePos} ({guiComponent.X},{guiComponent.Y},{guiComponent.Width},{guiComponent.Height}) {guiComponent.WorldBounds.ToStringSize()}");

                    if (!isHovered && hoveredComponents.Contains(guiComponent)) {
                        hoveredComponents.Remove(guiComponent);
                        guiComponent.InteractionState = GUIComponentState.None;

                        if (mouseDown) {
                            guiComponent.InvokeMouseReleased(mousePos.x, mousePos.y);
                        }

                        guiComponent.InvokeMouseExited(mousePos.x, mousePos.y);
                    } else if (isHovered && !hoveredComponents.Contains(guiComponent)) {
                        hoveredComponents.Add(guiComponent);

                        if (mouseDown) {
                            guiComponent.InteractionState = GUIComponentState.Clicked;
                            Focus = guiComponent;
                            guiComponent.InvokeMouseClicked(mousePos.x, mousePos.y);
                        } else {
                            guiComponent.InteractionState = GUIComponentState.Hovered;
                        }
                        guiComponent.InvokeMouseEntered(mousePos.x, mousePos.y);
                    } else if (isHovered) {
                        if (mouseDown)
                            guiComponent.InvokeMouseDown(mousePos.x, mousePos.y);
                        else
                            guiComponent.InvokeMouseHovering(mousePos.x, mousePos.y);
                    }
                }
            });
        }

        protected override void Update() {
            //Vector2 rawMousePosition = InputHandler.MousePosition;
            //Vector2 mousePosScreen = new Vector2(rawMousePosition);
            //Vector2 mousePosView = Scene.MainViewport.ScreenToViewport(new Vector2(rawMousePosition));
            //Vector2 mousePosWorld = Scene.MainViewport.ScreenToWorld(new Vector2(rawMousePosition));
            //Logging.Log.WriteLine(mousePosScreen + " " + mousePosView + " " + mousePosWorld);
        }

        public GUIComponent Focus {
            get => focus;
            set {
                if (focus != null && focus == value)
                    return;

                GUIComponent oldFocus = focus;

                focus = value;

                oldFocus?.InvokeOnFocusLost();

                focus?.InvokeOnFocusGained();
            }
        }
    }
}