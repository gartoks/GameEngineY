using System.Collections.Generic;
using GameEngine.Modding;

namespace GameEngine.Input {
    public static class Input {
        public static bool IsKeyPressed(Key key) => ModBase.InputManager.IsKeyPressed(key);

        public static bool IsKeyDown(Key key) => ModBase.InputManager.IsKeyDown(key);

        public static bool IsKeyReleased(Key key) => ModBase.InputManager.IsKeyReleased(key);

        public static IEnumerable<Key> PressedKeys => ModBase.InputManager.PressedKeys;

        public static IEnumerable<Key> DownKeys => ModBase.InputManager.DownKeys;

        public static IEnumerable<Key> ReleasedKeys => ModBase.InputManager.ReleasedKeys;

        public static bool IsMouseButtonPressed(MouseButton button) => ModBase.InputManager.IsMouseButtonPressed(button);

        public static bool IsMouseButtonDown(MouseButton button) => ModBase.InputManager.IsMouseButtonDown(button);

        public static bool IsMouseButtonReleased(MouseButton button) => ModBase.InputManager.IsMouseButtonReleased(button);

        public static IEnumerable<MouseButton> PressedMouseButtons => ModBase.InputManager.PressedMouseButtons;

        public static IEnumerable<MouseButton> DownMouseButtons => ModBase.InputManager.DownMouseButtons;

        public static IEnumerable<MouseButton> ReleasedMouseButtons => ModBase.InputManager.ReleasedMouseButtons;

        public static (int x, int y) MousePosition => ModBase.InputManager.MousePosition;

        public static (int dx, int dy) MouseMovement => ModBase.InputManager.MouseMovement;

        public static float MouseWheel => ModBase.InputManager.MouseWheel;

        public static float MouseWheelMovement => ModBase.InputManager.MouseWheelMovement;

        public static void AddKeyDownEventHandler(KeyDownEventHandler e) => ModBase.InputManager.AddKeyDownEventHandler(e);
        public static void AddKeyUpEventHandler(KeyUpEventHandler e) => ModBase.InputManager.AddKeyUpEventHandler(e);
        public static void AddKeyPressEventHandler(KeyPressEventHandler e) => ModBase.InputManager.AddKeyPressEventHandler(e);
        public static void AddMouseButtonDownEventHandler(MouseButtonDownEventHandler e) => ModBase.InputManager.AddMouseButtonDownEventHandler(e);
        public static void AddMouseButtonUpEventHandler(MouseButtonUpEventHandler e) => ModBase.InputManager.AddMouseButtonUpEventHandler(e);
        public static void AddMouseWheelEventHandler(MouseWheelEventHandler e) => ModBase.InputManager.AddMouseWheelEventHandler(e);
        public static void AddMouseMoveEventHandler(MouseMoveEventHandler e) => ModBase.InputManager.AddMouseMoveEventHandler(e);
        public static void RemoveKeyDownEventHandler(KeyDownEventHandler e) => ModBase.InputManager.RemoveKeyDownEventHandler(e);
        public static void RemoveKeyUpEventHandler(KeyUpEventHandler e) => ModBase.InputManager.RemoveKeyUpEventHandler(e);
        public static void RemoveKeyPressEventHandler(KeyPressEventHandler e) => ModBase.InputManager.RemoveKeyPressEventHandler(e);
        public static void RemoveMouseButtonDownEventHandler(MouseButtonDownEventHandler e) => ModBase.InputManager.RemoveMouseButtonDownEventHandler(e);
        public static void RemoveMouseButtonUpEventHandler(MouseButtonUpEventHandler e) => ModBase.InputManager.RemoveMouseButtonUpEventHandler(e);
        public static void RemoveMouseWheelEventHandler(MouseWheelEventHandler e) => ModBase.InputManager.RemoveMouseWheelEventHandler(e);
        public static void RemoveMouseMoveEventHandler(MouseMoveEventHandler e) => ModBase.InputManager.RemoveMouseMoveEventHandler(e);
    }
}