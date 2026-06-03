using Engine.Components.Graphics;
using Engine.ECS;
using Engine.Graphics;
using Engine.Helpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Sandbox.Scenes.TurnBasedCombatSample.Systems;

internal sealed class TargetingSystem
{
    private readonly Scene _scene;
    private readonly List<Entity> _targets;
    private readonly List<Entity> _targetRectEntities;

    public bool IsTargetingAll { get; private set; } = false;

    private int _currentTargetIndex = 0;

    public TargetingSystem(Scene scene, List<Entity> targets)
    {
        _scene = scene;
        _targets = targets;
        _targetRectEntities = [];
    }

    public void Initialize()
    {
        PrepareTargetRectEntities();
    }

    public void StartTargeting(bool targetAll = false)
    {
        _currentTargetIndex = 0;
        IsTargetingAll = targetAll;

        if (IsTargetingAll)
        {
            foreach (var targetRect in _targetRectEntities)
            {
                targetRect.IsVisible = true;
            }

            return;
        }

        _targetRectEntities[_currentTargetIndex].IsVisible = true;
    }

    public void StopTargeting()
    {
        foreach (var targetRect in _targetRectEntities)
        {
            targetRect.IsVisible = false;
        }
    }

    public void StartTargetingAll()
    {
        IsTargetingAll = true;
        foreach (var targetRect in _targetRectEntities)
        {
            targetRect.IsVisible = true;
        }
    }

    public void StopTargetingAll()
    {
        IsTargetingAll = false;
        foreach (var targetRect in _targetRectEntities)
        {
            targetRect.IsVisible = false;
        }

        _targetRectEntities[_currentTargetIndex].IsVisible = true;
    }

    public void NextTarget()
    {
        if (IsTargetingAll) return;

        _targetRectEntities[_currentTargetIndex].IsVisible = false;
        _currentTargetIndex = (_currentTargetIndex + 1) % _targets.Count;
        _targetRectEntities[_currentTargetIndex].IsVisible = true;
    }

    public void PreviousTarget()
    {
        if (IsTargetingAll) return;

        _targetRectEntities[_currentTargetIndex].IsVisible = false;
        _currentTargetIndex = (_currentTargetIndex - 1 + _targets.Count) % _targets.Count;
        _targetRectEntities[_currentTargetIndex].IsVisible = true;
    }

    public IEnumerable<Entity> GetCurrentTargets()
    {
        if (IsTargetingAll)
        {
            return _targets;
        }
        return [_targets[_currentTargetIndex]];
    }

    private void PrepareTargetRectEntities()
    {
        var rect = ShapesHelper.OutlinedRectangle(_scene.GraphicsDevice, 180, 180, 4, Color.Red);

        foreach (var target in _targets)
        {
            var sprite = new SpriteComponent(new Sprite(rect))
            {
                Origin = new Vector2(rect.Width / 2f, 0f)
            };
            var targetRectEntity = new Entity(target.Position)
                .AddComponent(sprite);

            _targetRectEntities.Add(targetRectEntity);
            targetRectEntity.IsVisible = false;

            _scene.AddEntity(targetRectEntity);
        }
    }
}
