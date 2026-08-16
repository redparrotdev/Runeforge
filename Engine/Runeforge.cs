using Engine.Coroutines;
using Engine.Debugging;
using Engine.ECS;
using Engine.Events;
using Engine.Inputs;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine;

public class Runeforge : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private bool _debugUIEnabled = false;

    public readonly RuneforgeSettings Settings;
    public readonly SceneManager SceneManager;
    public readonly DebugUI DebugUI;

    public Runeforge(RuneforgeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Settings = settings;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = settings.ContentFolder;
        IsMouseVisible = settings.IsMouseVisible;
        IsFixedTimeStep = settings.FixedFPS;
        TargetElapsedTime = TimeSpan.FromSeconds(1) / settings.TargetFPS;
        Window.Title = settings.Title;

        SceneManager = new SceneManager();
        DebugUI = new DebugUI(this);
    }

    public Runeforge() : this(RuneforgeSettings.Default)
    { 
    }

    public void SetScene(Scene newScene, bool savePreviosInNavStack = true)
    {
        SceneManager.SetScene(newScene, savePreviosInNavStack);
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Settings.Widht;
        _graphics.PreferredBackBufferHeight = Settings.Height;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Settings.CloseOnEscapeOrBack 
            && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
        {
            Exit();
        }

        CoroutineManager.Update(gameTime);
        InputManager.Update();
        EventManager.Update(gameTime);

        if (Settings.DebugEnabled && InputManager.Keyboard.KeyPressed(Keys.OemTilde))
        {
            _debugUIEnabled = !_debugUIEnabled;
        }

        if (_debugUIEnabled)
        {
            DebugUI.Update(gameTime);
        }

        SceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Settings.ClearColor);

        SceneManager.Draw(gameTime);

        if (_debugUIEnabled)
        {
            DebugUI.Draw(gameTime);
        }

        base.Draw(gameTime);
    }
}
