using Engine.ECS.Querying.Constraints;
using Engine.ECS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.ECS.Querying.Extensions;

public static class EntityQueryBuilderExtensions
{
    public static EntityQueryBuilder With<T>(this EntityQueryBuilder builder) where T : Component
    {
        var componentTypeId = ComponentType.GetId<T>();
        builder.AddConstraint(new WithComponentQueryConstraint(componentTypeId));

        return builder;
    }

    public static EntityQueryBuilder Without<T>(this EntityQueryBuilder builder) where T : Component
    {
        var componentTypeId = ComponentType.GetId<T>();
        builder.AddConstraint(new WithoutComponentQueryConstraint(componentTypeId));

        return builder;
    }

    public static EntityQueryBuilder WithAny(this EntityQueryBuilder builder, params Type[] componentTypes)
    {
        var componentTypeIds = componentTypes.Select(ComponentType.GetId);
        builder.AddConstraint(new WithAnyComponentQueryConstraint(componentTypeIds));

        return builder;
    }

    #region WithAny generic overloads

    public static EntityQueryBuilder WithAny<T1, T2>(this EntityQueryBuilder builder)
        where T1 : Component
        where T2 : Component
    {
        return builder.WithAny(typeof(T1), typeof(T2));
    }

    public static EntityQueryBuilder WithAny<T1, T2, T3>(this EntityQueryBuilder builder)
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        return builder.WithAny(typeof(T1), typeof(T2), typeof(T3));
    }

    #endregion

    public static EntityQueryBuilder WithAll(this EntityQueryBuilder builder, params Type[] componentTypes)
    {
        var componentTypeIds = componentTypes.Select(ComponentType.GetId);
        builder.AddConstraint(new WithAllComponentsQueryConstraint(componentTypeIds));

        return builder;
    }

    #region WithAll generic overloads

    public static EntityQueryBuilder WithAll<T1, T2>(this EntityQueryBuilder builder)
        where T1 : Component
        where T2 : Component
    {
        return builder.WithAll(typeof(T1), typeof(T2));
    }

    public static EntityQueryBuilder WithAll<T1, T2, T3>(this EntityQueryBuilder builder)
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        return builder.WithAll(typeof(T1), typeof(T2), typeof(T3));
    }

    #endregion

    #region Scene extensions

    public static IEnumerable<Entity> Query(this Scene scene, Action<EntityQueryBuilder> buildAction)
    {
        var builder = new EntityQueryBuilder();
        buildAction(builder);
        var query = builder.Build();

        return query.GetMatchingEntities(scene);
    }

    #endregion
}
