using Engine.Debugging;
using Engine.Debugging.Panels;
using Engine.Events;
using Engine.Inputs;
using Engine.Utils;
using Engine.ViewportAdapters;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using Sandbox.Debugging;
using Sandbox.Scenes;

namespace Sandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private bool _debugEnabled = false;
    private DebugUI _debugUI;
    private ViewportAdapter _viewportAdapter;
    private SceneManager _sceneManager;

    private EventManager.EventSubscription _debugChangeSceneEventSub;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        _debugUI = new DebugUI(this);
        Services.AddService(_debugUI);
        _debugUI.AddPanel(new RepformancePanel());

        _viewportAdapter = new ScaleViewportAdapter(GraphicsDevice, 1280, 720);
        Services.AddService(_viewportAdapter);

        _sceneManager = new SceneManager();
        Services.AddService(_sceneManager);

        var sceneSwitcher = new SceneSwitcherPanel();
        sceneSwitcher.AddScene("Sample scene", () => new SampleScene(this));
        sceneSwitcher.AddScene("Animation sample scene", () => new AnimationSampleScene(this));
        _debugUI.AddPanel(sceneSwitcher);

        _debugChangeSceneEventSub = EventManager.Subscribe<DebugChangeSceneEvent>(e =>
        {
            _sceneManager.SetScene(e.NewScene, e.SavePreviousSceneInNavigationStack);
        });

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void UnloadContent()
    {
        _debugChangeSceneEventSub.Unsubscribe();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        InputManager.Update();
        EventManager.Update(gameTime);

        if (InputManager.Keyboard.KeyPressed(Keys.OemTilde))
        {
            _debugEnabled = !_debugEnabled;
        }

        // TODO: Add your update logic here
        if (_debugEnabled)
        {
            _debugUI.Update(gameTime);
        }
        _sceneManager.Update(gameTime);

        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _sceneManager.Draw(gameTime);

        if (_debugEnabled)
        {
            _debugUI.Draw(gameTime);
        }

        base.Draw(gameTime);
    }
}
