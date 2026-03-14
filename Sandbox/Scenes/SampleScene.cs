using Engine.Components.Graphics;
using Engine.Debugging;
using Engine.Debugging.Panels;
using Engine.ECS;
using Engine.Graphics;
using Engine.Helpers;
using Engine.Inputs;
using Engine.Utils;
using Engine.ViewportAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sandbox.Scenes;

internal sealed class SampleScene : Scene
{
    private readonly EntitiesInspectorPanel _entitiesInspectorPanel;
    private Camera2DPanel _cameraPanel;

    private readonly VirtualButton _spawnEntityButton;

    private Camera2D _camera;
    private Texture2D _sampleTexture;

    public SampleScene(Game game) : base(game)
    {
        _entitiesInspectorPanel = new EntitiesInspectorPanel(Entities);

        _spawnEntityButton = new VirtualButton()
            .Keyboard(Keys.Space);
    }

    public override void Load()
    {
        var debugUI = Services.GetService<DebugUI>();
        debugUI.AddPanel(_entitiesInspectorPanel);

        var viewportAdapter = Services.GetService<ViewportAdapter>();
        _camera = new Camera2D(viewportAdapter);
        
        _cameraPanel = new Camera2DPanel(_camera);
        debugUI.AddPanel(_cameraPanel);

        _sampleTexture = ShapesHelper.Circle(GraphicsDevice, 32, Color.Red);
    }

    public override void Unload()
    {
        var debugUI = Services.GetService<DebugUI>();
        debugUI.RemovePanel(_entitiesInspectorPanel);
        debugUI.RemovePanel(_cameraPanel);
    }

    public override void Update(GameTime gameTime)
    {
        if (_spawnEntityButton.Pressed())
        {
            var entity = new Entity();
            var spriteComponent = new SpriteComponent(new Sprite(_sampleTexture));
            entity.AddComponent(spriteComponent);

            AddEntity(entity);
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch.Begin(transformMatrix: _camera.Matrix);
        Entities.Draw(SpriteBatch, gameTime);
        SpriteBatch.End();
    }
}
