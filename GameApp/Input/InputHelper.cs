using GameEngine.Input;

namespace GameApp.Input {
    internal static class InputHelper {
        internal static OpenTK.Input.Key ToOpenTKKey(Key key) {
            return (OpenTK.Input.Key)((int)key);
        }

        internal static KeyModifiers FromOpenTK(OpenTK.Input.KeyModifiers modifiers) {
            return (KeyModifiers)((int)modifiers);
        }

        internal static OpenTK.Input.KeyModifiers ToOpenTK(KeyModifiers modifiers) {
            return (OpenTK.Input.KeyModifiers)((int)modifiers);
        }

        internal static Key FromOpenTK(OpenTK.Input.Key key) {
            return (Key)((int)key);
        }

        internal static OpenTK.Input.MouseButton ToOpenTK(MouseButton button) {
            return (OpenTK.Input.MouseButton)((int)button);
        }

        internal static MouseButton FromOpenTK(OpenTK.Input.MouseButton button) {
            return (MouseButton)((int)button);
        }

        internal static OpenTK.Input.JoystickButton ToOpenTKButton(JoystickButton button) {
            return (OpenTK.Input.JoystickButton)((int)button);
        }

        internal static JoystickButton FromOpenTKButton(OpenTK.Input.JoystickButton button) {
            return (JoystickButton)((int)button);
        }

        internal static OpenTK.Input.JoystickAxis ToOpenTKAxis(JoystickAxis axis) {
            return (OpenTK.Input.JoystickAxis)((int)axis);
        }

        internal static JoystickAxis FromOpenTKAxis(OpenTK.Input.JoystickAxis axis) {
            return (JoystickAxis)((int)axis);
        }

        internal static OpenTK.Input.JoystickHat ToOpenTKHat(JoystickHat hat) {
            return (OpenTK.Input.JoystickHat)((int)hat);
        }

        internal static JoystickHat FromOpenTKHat(OpenTK.Input.JoystickHat hat) {
            return (JoystickHat)((int)hat);
        }

        internal static OpenTK.Input.Buttons ToOpenTK(GamepadButton button) {
            return (OpenTK.Input.Buttons)((int)button);
        }

        internal static GamepadButton FromOpenTK(OpenTK.Input.Buttons button) {
            return (GamepadButton)((int)button);
        }
    }
}