
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameApp.Logging;
using GameEngine.Input;
using GameEngine.Math;

namespace GameApp.Input {
    internal sealed class InputManager : IInputManager {
        private static InputManager instance;
        internal static InputManager Instance {
            get => InputManager.instance;
            private set { if (InputManager.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private readonly HashSet<Key> pressedKeys;
        private HashSet<Key> downKeys;
        private readonly HashSet<Key> releasedKeys;

        private readonly HashSet<MouseButton> pressedMouseButtons;
        private HashSet<MouseButton> downMouseButtons;
        private readonly HashSet<MouseButton> releasedMouseButtons;

        private readonly Vector2 mousePos;
        private Vector2 prevMousePos;
        private float mouseWheel;
        private float prevMouseWheel;

        private HashSet<Key> tmpDownKeys = new HashSet<Key>();
        private HashSet<MouseButton> tmpDownMouseButtons = new HashSet<MouseButton>();

        private event KeyDownEventHandler OnKeyDown;
        private event KeyUpEventHandler OnKeyUp;
        private event KeyPressEventHandler OnKeyPress;
        private event MouseButtonDownEventHandler OnMouseDown;
        private event MouseButtonUpEventHandler OnMouseUp;
        private event MouseWheelEventHandler OnMouseWheel;
        private event MouseMoveEventHandler OnMouseMove;

        internal InputManager() {
            Instance = this;

            this.mousePos = new Vector2();
            this.prevMousePos = new Vector2();

            this.pressedKeys = new HashSet<Key>();
            this.downKeys = new HashSet<Key>();
            this.releasedKeys = new HashSet<Key>();

            this.pressedMouseButtons = new HashSet<MouseButton>();
            this.downMouseButtons = new HashSet<MouseButton>();
            this.releasedMouseButtons = new HashSet<MouseButton>();
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            Window.Window.Instance.GameWindow.KeyDown += (sender, args) => {
                if (!args.IsRepeat)
                    OnKeyDown?.Invoke(InputHelper.FromOpenTK(args.Key), InputHelper.FromOpenTK(args.Modifiers));
            };
            Window.Window.Instance.GameWindow.KeyUp += (sender, args) => {
                if (!args.IsRepeat)
                    OnKeyUp?.Invoke(InputHelper.FromOpenTK(args.Key), InputHelper.FromOpenTK(args.Modifiers));
            };
            Window.Window.Instance.GameWindow.KeyPress += (sender, args) => OnKeyPress?.Invoke(args.KeyChar);
            Window.Window.Instance.GameWindow.MouseDown += (sender, args) => OnMouseDown?.Invoke(args.X, args.Y, InputHelper.FromOpenTK(args.Button));
            Window.Window.Instance.GameWindow.MouseUp += (sender, args) => OnMouseUp?.Invoke(args.X, args.Y, InputHelper.FromOpenTK(args.Button));  // TODO make sure no screenToWindow neccessary fpr mouse pos
            Window.Window.Instance.GameWindow.MouseWheel += (sender, args) => OnMouseWheel?.Invoke(args.X, args.Y, args.ValuePrecise, args.DeltaPrecise);
            Window.Window.Instance.GameWindow.MouseMove += (sender, args) => OnMouseMove?.Invoke(args.X, args.Y, args.XDelta, args.YDelta);
        }

        internal void Update() {
            OpenTK.Input.MouseState state = OpenTK.Input.Mouse.GetCursorState();
            Point mousePos = Window.Window.Instance.ScreenToWindow(state.X, state.Y);
            this.prevMousePos.Set(MousePosition);
            this.mousePos.Set(mousePos.X, mousePos.Y);

            this.prevMouseWheel = this.mouseWheel;
            this.mouseWheel = state.WheelPrecise;


            #region KeyHandling
            this.pressedKeys.Clear();
            this.releasedKeys.Clear();

            this.tmpDownKeys.Clear();

            foreach (Key key in GetDownKeys()) {
                if (!this.downKeys.Contains(key))
                    this.pressedKeys.Add(key);
                this.tmpDownKeys.Add(key);
            }

            foreach (Key key in this.downKeys.Except(this.tmpDownKeys)) {
                releasedKeys.Add(key);
            }

            HashSet<Key> swapkey = this.downKeys;
            this.downKeys = tmpDownKeys;
            this.tmpDownKeys = swapkey;
            #endregion KeyHandling

            #region MouseButtonHandling
            this.pressedMouseButtons.Clear();
            this.releasedMouseButtons.Clear();

            this.tmpDownMouseButtons.Clear();

            foreach (MouseButton button in GetDownMouseButtons()) {
                if (!this.downMouseButtons.Contains(button))
                    this.pressedMouseButtons.Add(button);
                this.tmpDownMouseButtons.Add(button);
            }

            foreach (MouseButton button in this.downMouseButtons.Except(this.tmpDownMouseButtons)) {
                releasedMouseButtons.Add(button);
            }

            HashSet<MouseButton> swapMouseButton = this.downMouseButtons;
            this.downMouseButtons = tmpDownMouseButtons;
            this.tmpDownMouseButtons = swapMouseButton;
            #endregion MouseButtonHandling
        }

        public bool IsKeyPressed(Key key) => this.pressedKeys.Contains(key);

        public bool IsKeyDown(Key key) => this.downKeys.Contains(key);

        public bool IsKeyReleased(Key key) => this.releasedKeys.Contains(key);

        public IEnumerable<Key> PressedKeys => this.pressedKeys;

        public IEnumerable<Key> DownKeys => this.downKeys;

        public IEnumerable<Key> ReleasedKeys => this.releasedKeys;

        public bool IsMouseButtonPressed(MouseButton button) => this.pressedMouseButtons.Contains(button);

        public bool IsMouseButtonDown(MouseButton button) => this.downMouseButtons.Contains(button);

        public bool IsMouseButtonReleased(MouseButton button) => this.releasedMouseButtons.Contains(button);

        public IEnumerable<MouseButton> PressedMouseButtons => this.pressedMouseButtons;

        public IEnumerable<MouseButton> DownMouseButtons => this.downMouseButtons;

        public IEnumerable<MouseButton> ReleasedMouseButtons => this.releasedMouseButtons;

        public Vector2 MousePosition => new Vector2(this.mousePos.x, this.mousePos.y);

        public Vector2 MouseMovement => MousePosition.Subtract(this.prevMousePos);

        public float MouseWheel => this.mouseWheel;

        public float MouseWheelMovement => this.prevMouseWheel - this.MouseWheel;

        public void AddKeyDownEventHandler(KeyDownEventHandler e) => OnKeyDown += e;

        public void AddKeyUpEventHandler(KeyUpEventHandler e) => OnKeyUp += e;

        public void AddKeyPressEventHandler(KeyPressEventHandler e) => OnKeyPress += e;

        public void AddMouseButtonDownEventHandler(MouseButtonDownEventHandler e) => OnMouseDown += e;

        public void AddMouseButtonUpEventHandler(MouseButtonUpEventHandler e) => OnMouseUp += e;

        public void AddMouseWheelEventHandler(MouseWheelEventHandler e) => OnMouseWheel += e;

        public void AddMouseMoveEventHandler(MouseMoveEventHandler e) => OnMouseMove += e;

        public void RemoveKeyDownEventHandler(KeyDownEventHandler e) => OnKeyDown -= e;

        public void RemoveKeyUpEventHandler(KeyUpEventHandler e) => OnKeyUp -= e;

        public void RemoveKeyPressEventHandler(KeyPressEventHandler e) => OnKeyPress -= e;

        public void RemoveMouseButtonDownEventHandler(MouseButtonDownEventHandler e) => OnMouseDown -= e;

        public void RemoveMouseButtonUpEventHandler(MouseButtonUpEventHandler e) => OnMouseUp -= e;

        public void RemoveMouseWheelEventHandler(MouseWheelEventHandler e) => OnMouseWheel -= e;

        public void RemoveMouseMoveEventHandler(MouseMoveEventHandler e) => OnMouseMove -= e;

        private static readonly HashSet<Key> internal_downKeys = new HashSet<Key>();
        private static IEnumerable<Key> GetDownKeys() {
            OpenTK.Input.KeyboardState state = OpenTK.Input.Keyboard.GetState();

            internal_downKeys.Clear();
            foreach (OpenTK.Input.Key key in Enum.GetValues(typeof(OpenTK.Input.Key))) {
                if (state.IsKeyDown(key))
                    internal_downKeys.Add(InputHelper.FromOpenTK(key));
            }

            return internal_downKeys;
        }

        private static readonly HashSet<MouseButton> internal_downMouseButtons = new HashSet<MouseButton>();
        private static IEnumerable<MouseButton> GetDownMouseButtons() {
            OpenTK.Input.MouseState state = OpenTK.Input.Mouse.GetCursorState();

            internal_downMouseButtons.Clear();
            foreach (OpenTK.Input.MouseButton b in Enum.GetValues(typeof(OpenTK.Input.MouseButton))) {
                if (state.IsButtonDown(b))
                    internal_downMouseButtons.Add(InputHelper.FromOpenTK(b));
            }

            return internal_downMouseButtons;
        }

        //private static HashSet<JoystickButton> internal_downJoystickButtons = new HashSet<JoystickButton>();
        //private static IEnumerable<JoystickButton> GetDownJoystickButtons() {
        //    internal_downJoystickButtons.Clear();
        //    JoystickCapabilities cap = Joystick.GetCapabilities(0);

        //    if (!cap.IsConnected)
        //        return internal_downJoystickButtons;

        //    JoystickState state = Joystick.GetState(0);

        //    foreach (OpenTK.Input.JoystickButton b in Enum.GetValues(typeof(OpenTK.Input.JoystickButton))) {
        //        if (state. IsButtonDown(b))
        //            internal_downMouseButtons.Add(MouseButtonUtils.FromOpenTK(b));
        //    }

        //    return internal_downMouseButtons;
        //}

    }
}