using Engine.Components.Graphics;
using Engine.Debugging;
using Engine.ECS;
using Engine.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Sandbox.Component;

internal class MovementComponent : UpdateComponent
{
    [DebugExpose]
    public float Speed = 250f;

    private readonly VirtualButton _leftButton;
    private readonly VirtualButton _rightButton;
    private readonly VirtualButton _upButton;
    private readonly VirtualButton _downButton;

    private List<SpriteComponent> _entitySprites = [];

    public MovementComponent()
    {
        _leftButton = new VirtualButton()
            .Keyboard(Keys.A);

        _rightButton = new VirtualButton()
            .Keyboard(Keys.D);

        _upButton = new VirtualButton()
            .Keyboard(Keys.W);

        _downButton = new VirtualButton()
            .Keyboard(Keys.S);
    }

    public override void OnAddedToEntity(Entity entity)
    {
        base.OnAddedToEntity(entity);

        _entitySprites = [..entity.GetAllComponents<SpriteComponent>()];
    }

    public override void Update(GameTime gameTime)
    {
        var direction = Vector2.Zero;

        if (_leftButton.Down())
        {
            direction.X -= 1;
        }
        if (_rightButton.Down())
        {
            direction.X += 1;
        }
        if (_upButton.Down())
        {
            direction.Y -= 1;
        }
        if (_downButton.Down())
        {
            direction.Y += 1;
        }

        if (direction.X < 0)
        {
            SetSpritesEffect(SpriteEffects.FlipHorizontally);
        }
        if (direction.X > 0)
        {
            SetSpritesEffect(SpriteEffects.None);
        }

        Entity.Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    private void SetSpritesEffect(SpriteEffects effect)
    {
        foreach (var sprite in _entitySprites)
        {
            sprite.SpriteEffect = effect;
        }
    }
}
