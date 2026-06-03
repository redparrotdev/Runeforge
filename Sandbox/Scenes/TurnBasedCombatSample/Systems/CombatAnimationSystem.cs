using Engine.Coroutines;
using Engine.ECS;
using Engine.ECS.Querying;
using Engine.ECS.Querying.Extensions;
using Sandbox.Scenes.TurnBasedCombatSample.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Scenes.TurnBasedCombatSample.Systems;

internal sealed class CombatAnimationSystem
{
    public bool IsWaitingForAnimation { get; private set; } = false;

    private readonly Scene _scene;
    private readonly EntityQueryBuilder.EntityQuery _entityQuery;

    private ICoroutine _waitRoutine;

    public CombatAnimationSystem(Scene scene)
    {
        _scene = scene;

        _entityQuery = new EntityQueryBuilder()
            .With<CombatAnimationPlayerComponent>()
            .Build();
    }

    public void WaitForAnimations()
    {
        _waitRoutine?.Stop();

        IsWaitingForAnimation = true;

        var entities = _entityQuery.GetMatchingEntities(_scene).ToArray();
        if (entities.Length == 0)
        {
            IsWaitingForAnimation = false;
            return;
        }

        var coroutines = entities
            .Select(e => e.GetComponent<CombatAnimationPlayerComponent>().CurrentAnimationCoroutine)
            .Where(c => c is not null)
            .ToArray();

        _waitRoutine = Coroutine.Start(WaitForAllAnimations(coroutines));
    }

    private IEnumerator WaitForAllAnimations(IEnumerable<ICoroutine> coroutines)
    {
        while (coroutines.Any(c => !c.Done))
        {
            yield return null;
        }

        IsWaitingForAnimation = false;
    }
}
