using Engine.Components.Graphics;
using Engine.Coroutines;
using Engine.ECS;
using System;
using System.Collections;
using System.Diagnostics;

namespace Sandbox.Scenes.TurnBasedCombatSample.Components;

internal class CombatAnimationPlayerComponent : Component
{
    public ICoroutine CurrentAnimationCoroutine;

    private SpriteAnimatorComponent _animator;
    private CombatStatsComponent _stats;

    private string _idleAnimation = "idle";
    private string _hurnAnimation = "hurt";
    private string _healAnimation = "heal";

    public override void OnAddedToEntity(Entity entity)
    {
        base.OnAddedToEntity(entity);

        _animator = entity.GetComponent<SpriteAnimatorComponent>();
        Debug.Assert(_animator is not null);

        _stats = entity.GetComponent<CombatStatsComponent>();
        Debug.Assert(_stats is not null);

        if (_stats is not null)
        {
            _stats.OnDamageTaken += HandleDamageTaken;
            _stats.OnHealed += HandleHealing;
        }
    }

    public override void OnRemovedFromEntity(Entity entity)
    {
        if (_stats is not null)
        {
            _stats.OnDamageTaken -= HandleDamageTaken;
            _stats.OnHealed -= HandleHealing;
        }

        base.OnRemovedFromEntity(entity);
    }

    #region Fluent setters

    public CombatAnimationPlayerComponent SetIdleAnimation(string animationName)
    {
        _idleAnimation = animationName;
        return this;
    }

    public CombatAnimationPlayerComponent SetHurtAnimation(string animationName)
    {
        _hurnAnimation = animationName;
        return this;
    }

    public CombatAnimationPlayerComponent SetHealAnimation(string animationName)
    {
        _healAnimation = animationName;
        return this;
    }

    #endregion

    private void HandleDamageTaken(CombatStatsComponent stats, int damage)
    {
        CurrentAnimationCoroutine?.Stop();

        CurrentAnimationCoroutine = Coroutine.Start(PlayAnimationAndReturnToIdle(_hurnAnimation));
    }

    private void HandleHealing(CombatStatsComponent stats, int amount)
    {
        CurrentAnimationCoroutine?.Stop();

        CurrentAnimationCoroutine = Coroutine.Start(PlayAnimationAndReturnToIdle(_healAnimation));
    }

    private IEnumerator PlayAnimationAndReturnToIdle(string animationName, Action callback = null)
    {
        if (_animator is null) yield break;

        _animator.Play(animationName, SpriteAnimatorComponent.LoopMode.OnceClamp);
        yield return new WaitForCondition(() => _animator.CurrentAnimationState == SpriteAnimatorComponent.AnimationState.Completed);

        _animator.Play(_idleAnimation);
        callback?.Invoke();

        CurrentAnimationCoroutine = null;
    }
}
