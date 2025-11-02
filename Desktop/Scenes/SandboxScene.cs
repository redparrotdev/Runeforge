using Desktop.Entities;
using Engine.Abstractions.Graphics;
using Engine.Graphics;
using Engine.Helpers.Graphics;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Desktop.Scenes;
internal class SandboxScene : Scene
{
    private Party _party;
    private ISprite _partyDebugSprite;

    private Queue<PartyEvent> _partyEvents = [];
    private PartyEvent? _currentEvent;

    private float _eventMovementSpeed = 180f;
    private Vector2 _eventDirection = new Vector2(-1, 0);
    private bool _isSelectingAction = false;

    public SandboxScene(Game game) : base(game)
    {
    }

    public override void Load()
    {
        _party = new Party([]);
        var debugTexture = ShapesHelper.OutlinedRectangle(
            SpriteBatch.GraphicsDevice
            , Party.ColliderWidth
            , Party.ColliderHeight
            , 2
            , Color.Red);

        _partyDebugSprite = new Sprite(debugTexture)
            .CenterOrigin();

        LoadEvents();
        NextEvent();
    }

    private void LoadEvents()
    {
        var greenRect = ShapesHelper.Rectangle(
            SpriteBatch.GraphicsDevice
            , 96
            , 128
            , Color.Green);

        var blueRect = ShapesHelper.Rectangle(
            SpriteBatch.GraphicsDevice
            , 96
            , 128
            , Color.Blue);

        var redRect = ShapesHelper.Rectangle(
            SpriteBatch.GraphicsDevice
            , 96
            , 128
            , Color.Red);

        var greenSprite = new Sprite(greenRect).CenterOrigin();
        var blueSprite = new Sprite(blueRect).CenterOrigin();
        var redSprite = new Sprite(redRect).CenterOrigin();

        var greenEvent = new PartyEvent("Green Event", greenSprite);
        var blueEvent = new PartyEvent("Blue Event", blueSprite);
        var redEvent = new PartyEvent("Red Event", redSprite);

        _partyEvents.Enqueue(greenEvent);
        _partyEvents.Enqueue(blueEvent);
        _partyEvents.Enqueue(redEvent);
    }

    public override void Update(GameTime gameTime)
    {
        if (_currentEvent != null && !_isSelectingAction)
        {
            var eventNewPosition = 
                _currentEvent.Position + _eventDirection * _eventMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentEvent.Position = eventNewPosition;
            _currentEvent.Update(gameTime);
        }

        if (IsEventAtParty())
        {
            NextEvent();
        }
    }

    public bool IsEventAtParty()
    {
        if (_currentEvent == null) return false;
        var eventCollider = _currentEvent.Collider;
        var partyCollider = _party.Collider;
        return partyCollider.CollideWith(eventCollider);
    }

    private void NextEvent()
    {
        if (_partyEvents.Count == 0)
        {
            _currentEvent = null;
            return;
        }

        _currentEvent = _partyEvents.Dequeue();
        _currentEvent.Position = new Vector2(
            Core.Viewport.Width + _eventMovementSpeed
            , _party.Collider.Position.Y);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch.Begin(samplerState: SamplerState.LinearClamp);
        SpriteBatch.GraphicsDevice.Clear(Color.White);

        _partyDebugSprite.Draw(SpriteBatch, _party.Collider.Position);
        _currentEvent?.Draw(SpriteBatch, gameTime);

        SpriteBatch.End();
    }
}
