using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.ECS.Utils;

public static class ComponentType
{
    /// <summary>
    /// Gets or sets the maximum number of distinct component types that can be registered in the system.
    /// Set the value before any registration of component types occurs. 
    /// Do not change this value after component types have been registered, as it may lead to undefined behavior.
    /// </summary>
    public static int MaxComponentTypes { get; set; } = 64;

    private static int _nextTypeId = 0;
    private static readonly Dictionary<Type, int> _typeIds = new();

    public static BitArray IssueSignature()
    {
        return new BitArray(MaxComponentTypes);
    }

    public static int GetId<T>() where T : Component
    {
        var type = typeof(T);
        
        return GetId(type);
    }

    public static int GetId(Type type)
    {
        Debug.Assert(type.IsAssignableTo(typeof(Component)));

        if (!_typeIds.TryGetValue(type, out var id))
        {
            id = _nextTypeId++;
            if (_nextTypeId > MaxComponentTypes)
            {
                throw new InvalidOperationException($"Cannot register more than {MaxComponentTypes} component types.");
            }

            _typeIds[type] = id;
        }

        return id;
    }
}
