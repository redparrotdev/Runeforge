using System.Collections.Generic;

namespace Sandbox.Scenes.TurnBasedCombatSample.Data;

internal sealed class Parameters : Dictionary<string, string>;

internal sealed class AbilityDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<AbilityCost> Costs { get; set; } = [];
    public TargetDefinition Target { get; set; }
    public List<EffectDefinition> Effects { get; set; } = [];

    public Parameters VisualParameters { get; set; } = [];
    public List<string> Tags { get; set; } = [];

    public Parameters AbilityParameters { get; set; } = [];
}

internal sealed class AbilityCost
{
    public string Type { get; set; }
    public int Amount { get; set; }

    public Parameters Parameters { get; set; } = [];
}

internal sealed class TargetDefinition
{
    public string Type { get; set; }
    public Parameters Parameters { get; set; } = [];
}

internal sealed class EffectDefinition
{
    public string Type { get; set; }
    public string Target { get; set; }

    public Parameters Parameters { get; set; } = [];
}
