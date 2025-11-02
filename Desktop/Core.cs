using Engine.Scenes;
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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Scene _scene;

        public Core()
        {
#pragma warning disable S3010 // Static fields should not be updated in constructors
            _instance = this;
#pragma warning restore S3010 // Static fields should not be updated in constructors

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            _scene = new Scenes.SandboxScene(this);

            base.Initialize();

            _scene.Load();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            _scene.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
