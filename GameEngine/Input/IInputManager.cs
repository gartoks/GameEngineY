using System.Collections.Generic;
using GameEngine.Math;

namespace GameEngine.Input {

    public delegate void KeyDownEventHandler(Key key, KeyModifiers modifiers);
    public delegate void KeyUpEventHandler(Key key, KeyModifiers modifiers);
    public delegate void KeyPressEventHandler(char keyChar);
    public delegate void MouseButtonDownEventHandler(int x, int y, MouseButton button);
    public delegate void MouseButtonUpEventHandler(int x, int y, MouseButton button);
    public delegate void MouseWheelEventHandler(int x, int y, float value, float valueChange);
    public delegate void MouseMoveEventHandler(int x, int y, int dx, int dy);

    public interface IInputManager {
        /// <summary>
        /// Determines whether [is key pressed] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is key pressed] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyPressed(Key key);

        /// <summary>
        /// Determines whether [is key down] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is key down] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyDown(Key key);

        /// <summary>
        /// Determines whether [is key released] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is key released] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyReleased(Key key);

        /// <summary>
        /// Gets the pressed keys.
        /// </summary>
        /// <value>
        /// The pressed keys.
        /// </value>
        IEnumerable<Key> PressedKeys { get; }

        /// <summary>
        /// Gets down keys.
        /// </summary>
        /// <value>
        /// Down keys.
        /// </value>
        IEnumerable<Key> DownKeys { get; }

        /// <summary>
        /// Gets the released keys.
        /// </summary>
        /// <value>
        /// The released keys.
        /// </value>
        IEnumerable<Key> ReleasedKeys { get; }

        /// <summary>
        /// Determines whether [is mouse button pressed] [the specified button].
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>
        ///   <c>true</c> if [is mouse button pressed] [the specified button]; otherwise, <c>false</c>.
        /// </returns>
        bool IsMouseButtonPressed(MouseButton button);

        /// <summary>
        /// Determines whether [is mouse button down] [the specified button].
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>
        ///   <c>true</c> if [is mouse button down] [the specified button]; otherwise, <c>false</c>.
        /// </returns>
        bool IsMouseButtonDown(MouseButton button);

        /// <summary>
        /// Determines whether [is mouse button released] [the specified button].
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>
        ///   <c>true</c> if [is mouse button released] [the specified button]; otherwise, <c>false</c>.
        /// </returns>
        bool IsMouseButtonReleased(MouseButton button);

        /// <summary>
        /// Gets the pressed mouse buttons.
        /// </summary>
        /// <value>
        /// The pressed mouse buttons.
        /// </value>
        IEnumerable<MouseButton> PressedMouseButtons { get; }

        /// <summary>
        /// Gets down mouse buttons.
        /// </summary>
        /// <value>
        /// Down mouse buttons.
        /// </value>
        IEnumerable<MouseButton> DownMouseButtons { get; }

        /// <summary>
        /// Gets the released mouse buttons.
        /// </summary>
        /// <value>
        /// The released mouse buttons.
        /// </value>
        IEnumerable<MouseButton> ReleasedMouseButtons { get; }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>
        /// The mouse position.
        /// </value>
        Vector2 MousePosition { get; }

        /// <summary>
        /// Gets the mouse movement.
        /// </summary>
        /// <value>
        /// The mouse movement.
        /// </value>
        Vector2 MouseMovement { get; }

        /// <summary>
        /// Gets the mouse wheel.
        /// </summary>
        /// <value>
        /// The mouse wheel.
        /// </value>
        float MouseWheel { get; }

        /// <summary>
        /// Gets the mouse wheel movement.
        /// </summary>
        /// <value>
        /// The mouse wheel movement.
        /// </value>
        float MouseWheelMovement { get; }

        /// <summary>
        /// Adds the key down event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddKeyDownEventHandler(KeyDownEventHandler e);

        /// <summary>
        /// Adds the key up event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddKeyUpEventHandler(KeyUpEventHandler e);

        /// <summary>
        /// Adds the key press event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddKeyPressEventHandler(KeyPressEventHandler e);

        /// <summary>
        /// Adds the mouse button down event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddMouseButtonDownEventHandler(MouseButtonDownEventHandler e);

        /// <summary>
        /// Adds the mouse button up event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddMouseButtonUpEventHandler(MouseButtonUpEventHandler e);

        /// <summary>
        /// Adds the mouse wheel event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddMouseWheelEventHandler(MouseWheelEventHandler e);

        /// <summary>
        /// Adds the mouse move event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void AddMouseMoveEventHandler(MouseMoveEventHandler e);

        /// <summary>
        /// Removes the key down event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveKeyDownEventHandler(KeyDownEventHandler e);

        /// <summary>
        /// Removes the key up event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveKeyUpEventHandler(KeyUpEventHandler e);

        /// <summary>
        /// Removes the key press event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveKeyPressEventHandler(KeyPressEventHandler e);

        /// <summary>
        /// Removes the mouse button down event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveMouseButtonDownEventHandler(MouseButtonDownEventHandler e);

        /// <summary>
        /// Removes the mouse button up event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveMouseButtonUpEventHandler(MouseButtonUpEventHandler e);

        /// <summary>
        /// Removes the mouse wheel event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveMouseWheelEventHandler(MouseWheelEventHandler e);

        /// <summary>
        /// Removes the mouse move event handler.
        /// </summary>
        /// <param name="e">The e.</param>
        void RemoveMouseMoveEventHandler(MouseMoveEventHandler e);

    }
}