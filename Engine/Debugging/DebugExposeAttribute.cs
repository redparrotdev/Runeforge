using System;

namespace Engine.Debugging;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DebugExposeAttribute : Attribute
{
    public string Label { get; }

    public DebugExposeAttribute(string label = null)
    {
        Label = label;
    }
}
