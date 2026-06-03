using Engine.Components.Graphics;
using Engine.Coroutines;
using Engine.Coroutines.YieldInstructions;
using Engine.Debugging;
using Engine.Debugging.Panels;
using Engine.ECS;
using Engine.Graphics;
using Engine.Graphics.Extensions;
using Engine.Helpers;
using Engine.Inputs;
using Engine.Utils;
using Engine.ViewportAdapters;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Sandbox.Scenes.TurnBasedCombatSample.AbilityEffects;
using Sandbox.Scenes.TurnBasedCombatSample.Components;
using Sandbox.Scenes.TurnBasedCombatSample.Data;
using Sandbox.Scenes.TurnBasedCombatSample.Systems;
using Sandbox.Scenes.TurnBasedCombatSample.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Scenes.TurnBasedCombatSample;

internal sealed class TurnBasedCombatScene : Scene
{
    private static Random Random = new Random();

    private readonly SimpleStateMachine<ECombatState> _combatStateMachine;
    private readonly TurnSystem _turnSystem;
    private readonly AbilityEffectRegistry _effectRegistry;
    private readonly AbilityExecutor _abilityExecutor;
    private readonly CombatAnimationSystem _combatAnimationSystem;

    private readonly List<Entity> _playerTeam;
    private readonly List<Entity> _enemyTeam;

    private readonly TargetingSystem _enemyTargeting;

    private readonly EntitiesInspectorPanel _inspectorPanel;
    private Camera2D _camera;
    private SpriteAtlas _sharedAtlas;

    private bool _disablePlayerInput = false;
    private bool _isEnemyPerformingAction = false;

    public TurnBasedCombatScene(Game game) : base(game)
    {
        _combatStateMachine = new SimpleStateMachine<ECombatState>(ECombatState.CombatStart);
        _turnSystem = new TurnSystem(this);

        _effectRegistry = new AbilityEffectRegistry();
        _abilityExecutor = new AbilityExecutor(_effectRegistry);
        _combatAnimationSystem = new CombatAnimationSystem(this);

        _playerTeam = new List<Entity>(4);
        _enemyTeam = new List<Entity>(4);

        _enemyTargeting = new TargetingSystem(this, _enemyTeam);

        _inspectorPanel = new EntitiesInspectorPanel(Entities);
    }

    public override void Load()
    {
        base.Load();

        var testBtn = new Button()
        {
            X = 100f,
            Y = 100f,
            Text = "Hello!"
        };
        testBtn.AddToRoot();

        var debugUI = Services.GetService<DebugUI>();
        debugUI.AddPanel(_inspectorPanel);

        var vpa = Services.GetService<ViewportAdapter>();
        _camera = new Camera2D(vpa);

        _sharedAtlas = Content.LoadTextureAtlasFromXml("AnimationSample/SampleSheet.xml");

        AddDebugCombatObserver();
        AddBasicEffects();
        CreatePlayer();
        CreateEnemy("Goblin", 5, 3, 7, 25);
        CreateEnemy("Goblin", 5, 3, 7, 25);
        PlaceCombatants();

        _enemyTargeting.Initialize();

        _turnSystem.InitializeCombat();
    }

    public override void Unload()
    {
        base.Unload();

        var debugUI = Services.GetService<DebugUI>();
        debugUI.RemovePanel(_inspectorPanel);
    }

    public override void Update(GameTime gameTime)
    {
        UpdateCombatState();
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: _camera.Matrix);
        Entities.Draw(SpriteBatch, gameTime);
        SpriteBatch.End();
    }

    private void AddDebugCombatObserver()
    {
        var combatStateDebugComponent = new CombatStateDebugComponent(_combatStateMachine);

        var entity = new Entity("CombatDebugObserver")
            .AddComponent(combatStateDebugComponent);

        AddEntity(entity);
    }

    private void AddBasicEffects()
    {
        var damageEffect = new DamageAbilityEffect();
        _effectRegistry.RegisterEffect(damageEffect.EffectType, damageEffect);
    }

    private void CreatePlayer()
    {
        var statsComponent = new CombatStatsComponent(10, 8, 8, 100);
        var combatantComponent = new CombatantComponent(CombatantComponent.ETeam.Player);

        var idleAnimation = _sharedAtlas.GetAnimation("idle-animation");
        var attackAnimation = _sharedAtlas.GetAnimation("cast-animation");
        var hurtAnimation = _sharedAtlas.GetAnimation("hurt-animation");
        var animatorComponent = new SpriteAnimatorComponent()
        {
            Scale = new Vector2(4f)
        };
        animatorComponent.AddAnimation("idle", idleAnimation);
        animatorComponent.AddAnimation("attack", attackAnimation);
        animatorComponent.AddAnimation("hurt", hurtAnimation);
        animatorComponent.Play("idle");

        var entity = new Entity("Player")
            .AddComponent(statsComponent)
            .AddComponent(combatantComponent)
            .AddComponent(animatorComponent)
            .AddComponent(new CombatAnimationPlayerComponent())
            .AddComponent(new BaseAttackComponent());

        var atk = entity.GetComponent<BaseAttackComponent>();
        atk.AbilityDefinition.Effects.Add(new EffectDefinition
        {
            Type = "Damage",
            Target = "Target"
        });

        _playerTeam.Add(entity);
        AddEntity(entity);
    }

    private void CreateEnemy(string name, int attack, int defense, int speed, int hp)
    {
        speed = Random.Next(Math.Max(speed - 2, 0), speed + 2);
        var statsComponent = new CombatStatsComponent(attack, defense, speed, hp);
        var combatantComponent = new CombatantComponent(CombatantComponent.ETeam.Enemy);

        var idleAnimation = _sharedAtlas.GetAnimation("idle-animation");
        var attackAnimation = _sharedAtlas.GetAnimation("cast-animation");
        var hurtAnimation = _sharedAtlas.GetAnimation("hurt-animation");
        var animatorComponent = new SpriteAnimatorComponent()
        {
            Scale = new Vector2(4f),
            SpriteEffect = SpriteEffects.FlipHorizontally
        };
        animatorComponent.AddAnimation("idle", idleAnimation);
        animatorComponent.AddAnimation("attack", attackAnimation);
        animatorComponent.AddAnimation("hurt", hurtAnimation);
        animatorComponent.Play("idle");

        var entity = new Entity(name)
            .AddComponent(statsComponent)
            .AddComponent(combatantComponent)
            .AddComponent(animatorComponent)
            .AddComponent(new CombatAnimationPlayerComponent())
            .AddComponent(new BaseAttackComponent());

        _enemyTeam.Add(entity);
        AddEntity(entity);
    }

    private void PlaceCombatants()
    {
        const int baseOffsetX = 250;
        const int spacingX = 180;

        for (int i = 0; i < _playerTeam.Count; i++)
        {
            var player = _playerTeam[i];
            var x = -baseOffsetX - (spacingX * i);
            player.Position = new Vector2(x, player.Position.Y);
        }

        for (int i = 0; i < _enemyTeam.Count; i++)
        {
            var enemy = _enemyTeam[i];
            var x = baseOffsetX + (spacingX * i);
            enemy.Position = new Vector2(x, enemy.Position.Y);
        }
    }

    private void UpdateCombatState()
    {
        var currentState = _combatStateMachine.CurrentState;
        switch (currentState)
        {
            case ECombatState.CombatStart:
                _combatStateMachine.ChangeState(ECombatState.RoundStart);
                break;
            case ECombatState.RoundStart:
                _combatStateMachine.ChangeState(ECombatState.TurnStart);
                break;
            case ECombatState.TurnStart:
                UpdateNextCombatantTurn();
                break;
            case ECombatState.PlayerTurnStart:
                UpdatePlayerTurn();
                break;
            case ECombatState.EnemyTurnStart:
                UpdateEnemyTurn();
                break;
            case ECombatState.TurnEnd:
                UpdateTurnEnd();
                break;
            case ECombatState.RoundEnd:
                UpdateRoundEnd();
                break;
            case ECombatState.CombatEnd:
                UpdateCombatEnd();
                break;
        }
    }

    private void UpdateNextCombatantTurn()
    {
        var isPlayerTurn = _turnSystem.CurrentCombatant.GetComponent<CombatantComponent>().Team == CombatantComponent.ETeam.Player;
        if (isPlayerTurn)
        {
            _combatStateMachine.ChangeState(ECombatState.PlayerTurnStart);
            _enemyTargeting.StartTargeting();
            return;
        }

        _combatStateMachine.ChangeState(ECombatState.EnemyTurnStart);
    }

    private void UpdatePlayerTurn()
    {
        if (_disablePlayerInput) return;

        if (InputManager.Keyboard.KeyPressed(Keys.D))
        {
            _enemyTargeting.NextTarget();
        }
        if (InputManager.Keyboard.KeyPressed(Keys.A))
        {
            _enemyTargeting.PreviousTarget();
        }

        if (InputManager.Keyboard.KeyPressed(Keys.S))
        {
            if (_enemyTargeting.IsTargetingAll)
            {
                _enemyTargeting.StopTargetingAll();
            }
            else
            {
                _enemyTargeting.StartTargetingAll();
            }
        }

        if (!InputManager.Keyboard.KeyPressed(Keys.Space)) return;

        _disablePlayerInput = true;
        _enemyTargeting.StopTargeting();
        var animator = _turnSystem.CurrentCombatant.GetComponent<SpriteAnimatorComponent>();
        Coroutine.Start(WaitAndThen(
            WaitForAnimation(animator, "attack")
            , () =>
            {
                animator.Play("idle");
                _disablePlayerInput = false;

                var targets = _enemyTargeting.GetCurrentTargets();
                var caster = _turnSystem.CurrentCombatant;
                var baseAttack = caster.GetComponent<BaseAttackComponent>();
                _abilityExecutor.ExecuteAbility(baseAttack.AbilityDefinition, caster, targets, _playerTeam.Concat(_enemyTeam));

                _combatStateMachine.ChangeState(ECombatState.TurnEnd);
            }));
    }

    private void UpdateEnemyTurn()
    {
        if (_isEnemyPerformingAction) return;

        _isEnemyPerformingAction = true;

        Coroutine.Start(EnemyTurnRoutime(_turnSystem.CurrentCombatant));

        IEnumerator EnemyTurnRoutime(Entity enemy)
        {
            yield return new WaitForSeconds(0.2f);

            var animator = enemy.GetComponent<SpriteAnimatorComponent>();
            animator.Play("attack", SpriteAnimatorComponent.LoopMode.OnceClamp);
            yield return new WaitForCondition(() => animator.CurrentAnimationState == SpriteAnimatorComponent.AnimationState.Completed);
            animator.Play("idle");

            var randomPlayerIndex = Random.Next(_playerTeam.Count);
            var randomPlayer = _playerTeam[randomPlayerIndex];
            var baseAttack = enemy.GetComponent<BaseAttackComponent>();
            _abilityExecutor.ExecuteAbility(baseAttack.AbilityDefinition, enemy, [randomPlayer], _playerTeam.Concat(_enemyTeam));

            _combatStateMachine.ChangeState(ECombatState.TurnEnd);
            _isEnemyPerformingAction = false;
        }
    }

    private void UpdateTurnEnd()
    {
        if (_combatAnimationSystem.IsWaitingForAnimation) return;

        Coroutine.Start(EndTurnRoutine());

        IEnumerator EndTurnRoutine()
        {
            _combatAnimationSystem.WaitForAnimations();

            yield return new WaitForCondition(() => !_combatAnimationSystem.IsWaitingForAnimation);

            _turnSystem.NextTurn();
            if (_turnSystem.CurrentCombatant is not null)
            {
                _combatStateMachine.ChangeState(ECombatState.TurnStart);
            }
            else
            {
                _combatStateMachine.ChangeState(ECombatState.RoundEnd);
            }
        }
    }

    public void UpdateRoundEnd()
    {
        _turnSystem.NextRound();
        _combatStateMachine.ChangeState(ECombatState.RoundStart);
    }

    private void UpdateCombatEnd()
    {
        // Handle end of combat (e.g., show victory/defeat screen, rewards, etc.)
    }

    private static IEnumerator WaitForAnimation(SpriteAnimatorComponent animator, string animationName)
    {
        animator.Play(animationName, SpriteAnimatorComponent.LoopMode.OnceClamp);
        yield return new WaitForCondition(() => animator.CurrentAnimationState == SpriteAnimatorComponent.AnimationState.Completed);
    }

    private static IEnumerator WaitAndThen(IEnumerator routine, Action action)
    {
        yield return Coroutine.Start(routine);

        action();
    }
}

public sealed class WaitForCondition : IYieldInstruction
{
    private readonly Func<bool> _condition;

    public WaitForCondition(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool ShouldWait(GameTime gameTime)
    {
        return !_condition();
    }
}