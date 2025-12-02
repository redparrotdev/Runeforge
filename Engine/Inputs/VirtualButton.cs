using Engine.Inputs.Base;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Inputs;

public class VirtualButton : VirtualInput
{
    private readonly List<IButtonNode> _nodes = [];

    public VirtualButton(List<IButtonNode> nodes = null)
    {
        if (nodes != null)
        {
            _nodes.AddRange(nodes);
        }
    }

    public bool Down()
    {
        if (Disabled) return false;

        return _nodes.Any(n => n.ButtonDown());
    }

    public bool Up()
    {
        if (Disabled) return false;

        return _nodes.Any(n => n.ButtonUp());
    }

    public bool Pressed()
    {
        if (Disabled) return false;

        return _nodes.Any(n => n.ButtonPressed());
    }

    public bool Released()
    {
        if (Disabled) return false;

        return _nodes.Any(n => n.ButtonReleased());
    }

    #region Fluent methods

    public VirtualButton Keyboard(Keys key)
    {
        _nodes.Add(new KeyboardButton(key));

        return this;
    }

    public VirtualButton Mouse(Inputs.MouseButton btn)
    {
        _nodes.Add(new MouseButton(btn));

        return this;
    }

    #endregion

    public interface IButtonNode
    {
        bool ButtonDown();
        bool ButtonUp();
        bool ButtonPressed();
        bool ButtonReleased();
    }

    public sealed class KeyboardButton : IButtonNode
    {
        private readonly Keys _key;

        public KeyboardButton(Keys key)
        {
            _key = key;
        }

        public bool ButtonDown()
        {
            return InputManager.Keyboard.KeyDown(_key);
        }

        public bool ButtonPressed()
        {
            return InputManager.Keyboard.KeyPressed(_key);
        }

        public bool ButtonReleased()
        {
            return InputManager.Keyboard.KeyReleased(_key);
        }

        public bool ButtonUp()
        {
            return InputManager.Keyboard.KeyUp(_key);
        }
    }

    public sealed class MouseButton : IButtonNode
    {
        private readonly Inputs.MouseButton _button;

        public MouseButton(Inputs.MouseButton button)
        {
            _button = button;
        }

        public bool ButtonDown()
        {
            return _button switch
            {
                Inputs.MouseButton.Left => InputManager.Mouse.LeftDown(),
                Inputs.MouseButton.Right => InputManager.Mouse.RightDown(),
                _ => false
            };
        }

        public bool ButtonPressed()
        {
            return _button switch
            {
                Inputs.MouseButton.Left => InputManager.Mouse.LeftPressed(),
                Inputs.MouseButton.Right => InputManager.Mouse.RightPressed(),
                _ => false
            };
        }

        public bool ButtonReleased()
        {
            return _button switch
            {
                Inputs.MouseButton.Left => InputManager.Mouse.LeftReleased(),
                Inputs.MouseButton.Right => InputManager.Mouse.RightReleased(),
                _ => false
            };
        }

        public bool ButtonUp()
        {
            return _button switch
            {
                Inputs.MouseButton.Left => InputManager.Mouse.LeftUp(),
                Inputs.MouseButton.Right => InputManager.Mouse.LeftDown(),
                _ => false
            };
        }
    }
}
