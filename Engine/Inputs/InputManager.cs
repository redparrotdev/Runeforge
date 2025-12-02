using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Inputs;

using MGKeyboard = Microsoft.Xna.Framework.Input.Keyboard;
using MGMouse = Microsoft.Xna.Framework.Input.Mouse;

public static class InputManager
{
    public static void Update()
    {
        Keyboard.UpdatState();
        Mouse.UpdateState();
    }

    public static class Keyboard
    {
        public static KeyboardState CurrentState { get; private set; }
        public static KeyboardState PreviousState { get; private set; }

        static Keyboard()
        {
            CurrentState = MGKeyboard.GetState();
            PreviousState = CurrentState;
        }

        internal static void UpdatState()
        {
            PreviousState = CurrentState;
            CurrentState = MGKeyboard.GetState();
        }

        public static bool KeyDown(Keys key) => CurrentState.IsKeyDown(key);
        public static bool KeyUp(Keys key) => CurrentState.IsKeyUp(key);
        public static bool KeyPressed(Keys key) => KeyDown(key) && PreviousState.IsKeyUp(key);
        public static bool KeyReleased(Keys key) => KeyUp(key) && PreviousState.IsKeyDown(key);
    }

    public static class Mouse
    {
        public static MouseState CurrentState { get; private set; }
        public static MouseState PreviousState { get; private set; }

        static Mouse()
        {
            CurrentState = MGMouse.GetState();
            PreviousState = CurrentState;
        }

        internal static void UpdateState()
        {
            PreviousState = CurrentState;
            CurrentState = MGMouse.GetState();
        }

        public static bool LeftDown() => CurrentState.LeftButton == ButtonState.Pressed;
        public static bool LeftUp() => CurrentState.LeftButton == ButtonState.Released;
        public static bool LeftPressed() => LeftDown() && PreviousState.LeftButton == ButtonState.Released;
        public static bool LeftReleased() => LeftUp() && PreviousState.LeftButton == ButtonState.Pressed;

        public static bool RightDown() => CurrentState.RightButton == ButtonState.Pressed;
        public static bool RightUp() => CurrentState.RightButton== ButtonState.Released;
        public static bool RightPressed() => RightDown() && PreviousState.RightButton == ButtonState.Released;
        public static bool RightReleased() => RightUp() && PreviousState.RightButton == ButtonState.Pressed;

        public static Vector2 Position() => CurrentState.Position.ToVector2();
    }
}
