using Desktop.Entities;
using Desktop.Events;
using Desktop.Screens.Encounters;
using Engine.Abstractions.Graphics;
using Engine.Graphics;
using Engine.Helpers.Graphics;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Engine.Events.EventManager;

namespace Desktop.Scenes;
internal class SandboxScene : Scene
{
    private Party _party;
    private ISprite _partyDebugSprite;

    private readonly Queue<PartyEvent> _partyEvents = [];
    private PartyEvent _currentEvent;

    private readonly float _eventMovementSpeed = 180f;
    private Vector2 _eventDirection = new Vector2(-1, 0);
    private bool _isSelectingAction = false;

    private EncounterScreen _encounterScreen;
    private EventSubscription _encounterScreenEventSub;

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

        _encounterScreen = new EncounterScreen(Content, SpriteBatch.GraphicsDevice)
            .SetEncounterText("There is an encounter!");
        _encounterScreenEventSub = Subscribe<GameEvents.EncounterOptionSelected>(d =>
        {
            _isSelectingAction = false;
            _encounterScreen.ClearOptions();
            NextEvent();
        });
    }

    public override void Unload()
    {
        _encounterScreenEventSub.Unsubscribe();
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
        if (_isSelectingAction)
        {
            _encounterScreen.Update(gameTime);
        }

        if (_currentEvent != null && !_isSelectingAction)
        {
            var eventNewPosition = 
                _currentEvent.Position + _eventDirection * _eventMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentEvent.Position = eventNewPosition;
            _currentEvent.Update(gameTime);
        }

        if (IsEventAtParty())
        {
            _isSelectingAction = true;
            _currentEvent.Collider.IsActive = false;
            Encounter();
        }
    }

    [MemberNotNullWhen(true, nameof(_currentEvent))]
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
            Core.ViewportAdapter.VirtualWidth + _eventMovementSpeed
            , _party.Collider.Position.Y);
    }

    private void Encounter()
    {
        if (_currentEvent == null) return;

        var (option1, option3, option2) = _currentEvent.Name switch
        {
            "Green Event" => ("Fight", null, "Run"),
            "Blue Event" => ("Talk", null, "Ignore"),
            "Red Event" => ("Bribe", "Use Item", "Steal"),
            _ => ("Option 1", null, "Option 2")
        };

        var encounterText = $"You have encountered a {_currentEvent.Name}!";
        _encounterScreen
            .SetEncounterText(encounterText)
            .SetFirstOption(new EncounterOption(option1))
            .SetSecondOption(new EncounterOption(option2))
            .SetThirdOption(option3 != null ? new EncounterOption(option3) : null);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch.GraphicsDevice.Clear(Color.White);
        SpriteBatch.Begin(samplerState: SamplerState.LinearClamp);

        _partyDebugSprite.Draw(SpriteBatch, _party.Collider.Position);
        _currentEvent?.Draw(SpriteBatch, gameTime);

        if (_isSelectingAction)
        {
            _encounterScreen.Draw(SpriteBatch, gameTime);
        }

        SpriteBatch.End();
    }
}
