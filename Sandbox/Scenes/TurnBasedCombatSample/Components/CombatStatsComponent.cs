using Engine.Debugging;
using Engine.ECS;
using System;

namespace Sandbox.Scenes.TurnBasedCombatSample.Components;

internal sealed class CombatStatsComponent : Component
{
    public event Action<CombatStatsComponent, int> OnDamageTaken;
    public event Action<CombatStatsComponent, int> OnHealed;

    [DebugExpose]
    public int Attack { get; set; }

    [DebugExpose]
    public int Defense { get; set; }

    [DebugExpose]
    public int Speeed { get; set; }

    [DebugExpose]
    public int MaxHp { get; set; }

    [DebugExpose]
    public int Hp { get; set; }

    public CombatStatsComponent(int attack, int defense, int speed, int hp)
    {
        Attack = attack;
        Defense = defense;
        Speeed = speed;
        MaxHp = hp;
        Hp = hp;
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(Hp - damage, 0);
        OnDamageTaken?.Invoke(this, damage);
    }

    public void Heal(int amount)
    {
        Hp = Math.Min(Hp + amount, MaxHp);
        OnHealed?.Invoke(this, amount);
    }
}
