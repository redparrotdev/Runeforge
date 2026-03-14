using Engine.Components.Graphics;
using Engine.Debugging;
using Engine.Debugging.Panels;
using Engine.ECS;
using Engine.Graphics;
using Engine.Graphics.Extensions;
using Engine.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sandbox.Scenes;

internal sealed class AnimationSampleScene : Scene
{
    private readonly EntitiesInspectorPanel _inspectorPanel;

    private SpriteAtlas _explosionAtlas;

    public AnimationSampleScene(Game game) : base(game)
    {
        _inspectorPanel = new EntitiesInspectorPanel(Entities);
    }

    public override void Load()
    {
        base.Load();

        var debugUI = Services.GetService<DebugUI>();
        debugUI.AddPanel(_inspectorPanel);

        AddSampleAnimatedEntity();
        _explosionAtlas = Content.LoadTextureAtlasFromXml("AnimationSample/Explosion.xml");
    }

    public override void Unload()
    {
        base.Unload();

        var debugUI = Services.GetService<DebugUI>();
        debugUI.RemovePanel(_inspectorPanel);
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Keyboard.KeyPressed(Keys.Space))
        {
            var mousePos = InputManager.Mouse.Position();
            CreateExplosionEntity(mousePos);
        }

        base.Update(gameTime);
    }

    private void AddSampleAnimatedEntity()
    {
        var atlas = Content.LoadTextureAtlasFromXml("AnimationSample/SampleSheet.xml");
        var idleAnimation = atlas.GetAnimation("idle-animation");
        var spriteAnimator = new SpriteAnimatorComponent()
        {
            Scale = new Vector2(4f)
        };
        spriteAnimator.AddAnimation("idle", idleAnimation);

        spriteAnimator.Play("idle");

        var entity = new Entity("Animated", new Vector2(250f))
            .AddComponent(spriteAnimator);

        AddEntity(entity);
    }

    private void CreateExplosionEntity(Vector2 position)
    {
        var animation = _explosionAtlas.GetAnimation("effect");
        var animator = new SpriteAnimatorComponent()
        {
            Scale = new Vector2(4f)
        };
        animator.AddAnimation("effect", animation);
        animator.Play("effect", SpriteAnimatorComponent.LoopMode.OnceClamp);
        animator.OnAnimationCompleted += (c, _) => c.Entity.Kill();

        var entity = new Entity(position)
            .AddComponent(animator);

        AddEntity(entity);
    }
}
