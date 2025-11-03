using Desktop.Extensions;
using Engine.Scenes;
using Engine.ViewportAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Desktop
{
    public class Core : Game
    {
        private static Core _instance;
        public static Core Instance => _instance;
        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static ViewportAdapter ViewportAdapter => Instance._viewportAdapter;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ViewportAdapter _viewportAdapter;
        private RenderTarget2D _sceneRenderTarget;

        private Scene _scene;

        public Core()
        {
#pragma warning disable S3010 // Static fields should not be updated in constructors
            _instance = this;
#pragma warning restore S3010 // Static fields should not be updated in constructors

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _viewportAdapter = new ScaleViewportAdapter(
                GraphicsDevice
                , 1280
                , 720);
            _sceneRenderTarget = new RenderTarget2D(
                GraphicsDevice
                , _viewportAdapter.VirtualWidth
                , _viewportAdapter.VirtualHeight);

            _scene = new Scenes.SandboxScene(this);

            base.Initialize();

            _scene.Load();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_sceneRenderTarget);
            _scene.Draw(gameTime);

            var transformMatrix = _viewportAdapter.GetScaleMatrix();

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
            _spriteBatch.Draw(_sceneRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
