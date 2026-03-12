using Engine.ECS;
using Engine.ECS.Utils;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Engine.Debugging.Panels;

public sealed class EntitiesInspectorPanel : IDebugPanel
{
    private static readonly ConcurrentDictionary<Type, MemberInfo[]> _componentsMembersCache = new();

    public string Name => $"Entities";

    public ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;

    private readonly EntitiesList _entities;

    public EntitiesInspectorPanel(EntitiesList entities)
    {
        _entities = entities;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime)
    {
        string entityNameRef = string.Empty;
        ImGui.InputTextWithHint(string.Empty, "Search by name", ref entityNameRef, 64);

        var entities = string.IsNullOrWhiteSpace(entityNameRef)
            ? _entities
            : _entities.Where(e => e.Name.Contains(entityNameRef, StringComparison.OrdinalIgnoreCase));

        ImGui.SameLine();
        ImGui.Text($"Count: {entities.Count()}");

        ImGui.BeginChild("EntitiesList", new System.Numerics.Vector2(0f), ImGuiChildFlags.AlwaysAutoResize);
        foreach (var entity in entities)
        {
            ImGui.PushID(entity.GetHashCode());
            if (ImGui.CollapsingHeader($"Entity: {entity.Name}"))
            {
                DrawEntityPosition(entity);
                DrawEntityStateCheckboxes(entity);

                foreach (var component in entity.Components)
                {
                    ImGui.PushID(HashCode.Combine(entity, component));

                    if (ImGui.CollapsingHeader($"{component.GetType().Name}"))
                    {
                        DrawComponentMembers(component);
                    }

                    ImGui.PopID();
                }
            }
            ImGui.PopID();
            ImGui.Separator();
        }
        ImGui.EndChild();
    }

    private static void DrawEntityPosition(Entity entity)
    {
        var entityPosRef = new System.Numerics.Vector2(entity.Position.X, entity.Position.Y);
        if (ImGui.InputFloat2("Position", ref entityPosRef, null, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            entity.Position = new Vector2(entityPosRef.X, entityPosRef.Y);
        }
    }

    private static void DrawEntityStateCheckboxes(Entity entity)
    {
        var isActiveRef = entity.IsActive;
        var isVisibleRef = entity.IsVisible;

        if (ImGui.Checkbox("Is Active", ref isActiveRef))
        {
            entity.IsActive = isActiveRef;
        }

        if (ImGui.Checkbox("Is Visible", ref isVisibleRef))
        {
            entity.IsVisible = isVisibleRef;
        }
    }

    private static void DrawComponentMembers(Component component)
    {
        var members = GetComponentExposedMembers(component);

        foreach (var member in members)
        {
            var attributeProvidedLabel = member.GetCustomAttribute<DebugExposeAttribute>()?.Label;
            var label = string.IsNullOrWhiteSpace(attributeProvidedLabel)
                ? member.Name
                : attributeProvidedLabel;

            var value = member.GetValue(component);

            if (member.IsReadonlyProperty())
            {
                ImGui.Text($"{label}: {value}");
                continue;
            }

            switch (value)
            {
                case int intValue:
                    DrawIntMember(component, member, label, intValue);
                    break;
                case float floatValue:
                    DrawFloatMember(component, member, label, floatValue);
                    break;
                case bool boolValue:
                    DrawBoolMember(component, member, label, boolValue);
                    break;
                case Vector2 vec2Value:
                    DrawVec2Member(component, member, label, vec2Value);
                    break;
                case Color colorValue:
                    DrawColorMember(component, member, label, colorValue);
                    break;
                case Enum enumValue:
                    var enumType = enumValue.GetType();
                    var isFlags = enumType.GetCustomAttribute<FlagsAttribute>() is not null;
                    if (isFlags)
                    {
                        DrawFlagsEnumMember(component, member, label, enumValue);
                    }
                    else
                    {
                        DrawCommonEnumMember(component, member, label, enumValue);
                    }
                    break;
                default:
                    ImGui.Text($"{label}: {value} (Unsupported debug type)");
                    break;
            }
        }
    }

    private static void DrawFloatMember(Component component, MemberInfo member, string label, float value)
    {
        if (ImGui.InputFloat(label, ref value, 1f, 5f, null, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            member.SetValue(component, value);
        }
    }

    private static void DrawIntMember(Component component, MemberInfo member, string label, int value)
    {
        if (ImGui.InputInt(label, ref value, 1, 5, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            member.SetValue(component, value);
        }
    }

    private static void DrawBoolMember(Component component, MemberInfo member, string label, bool value)
    {
        if (ImGui.Checkbox(label, ref value))
        {
            member.SetValue(component, value);
        }
    }

    private static void DrawVec2Member(Component component, MemberInfo member, string label, Vector2 value)
    {
        var vecRef = new System.Numerics.Vector2(value.X, value.Y);
        if (ImGui.InputFloat2(label, ref vecRef, null, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            member.SetValue(component, new Vector2(vecRef.X, vecRef.Y));
        }
    }

    private static void DrawColorMember(Component component, MemberInfo member, string label, Color value)
    {
        int[] valueRef = [value.R, value.G, value.B, value.A];
        if (ImGui.InputInt4(label, ref valueRef[0], ImGuiInputTextFlags.EnterReturnsTrue))
        {
            member.SetValue(component, new Color(valueRef[0], valueRef[1], valueRef[2], valueRef[3]));
        }
    }

    private static void DrawCommonEnumMember(Component component, MemberInfo member, string label, Enum value)
    {
        var enumType = value.GetType();
        var enumValues = Enum.GetValues(enumType);

        if (ImGui.BeginCombo(label, value.ToString()))
        {
            foreach (var item in enumValues)
            {
                bool selected = value.Equals(item);

                if (ImGui.Selectable(item.ToString(), selected))
                {
                    member.SetValue(component, item);
                }

                if (selected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }
    }

    private static void DrawFlagsEnumMember(Component component, MemberInfo member, string label, Enum value)
    {
        var enumType = value.GetType();
        var enumValues = Enum.GetValues(enumType);

        var flags = Convert.ToInt32(value);
        var flagsRef = flags;

        ImGui.Text(label);
        foreach (var item in enumValues)
        {
            var itemIntValue = Convert.ToInt32(item);
            if (itemIntValue == 0) continue;

            ImGui.CheckboxFlags(item.ToString(), ref flagsRef, itemIntValue);
        }

        if (flags != flagsRef)
        {
            var newValue = Enum.Parse(enumType, flagsRef.ToString(), true);
            member.SetValue(component, newValue);
        }
    }

    private static MemberInfo[] GetComponentExposedMembers(Component component)
    {
        var componentType = component.GetType();
        if (_componentsMembersCache.TryGetValue(componentType, out var cachedMembers))
        {
            return cachedMembers;
        }

#pragma warning disable S3011 // Bypass required by logic
        var debugExposedMembers = componentType
            .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(m => m.GetCustomAttribute<DebugExposeAttribute>() is not null)
            .OrderBy(m => m.DeclaringType != m.ReflectedType ? 0 : 1)
            .ToArray();
#pragma warning restore S3011

        _componentsMembersCache.TryAdd(componentType, debugExposedMembers);

        return debugExposedMembers;
    }
}

file static class Extensions
{
    public static object GetValue(this MemberInfo memberInfo, object obj)
    {
        if (memberInfo is FieldInfo field)
        {
            return field.GetValue(obj);
        }
        if (memberInfo is PropertyInfo property)
        {
            return property.GetValue(obj);
        }

        return null;
    }

    public static void SetValue(this MemberInfo memberInfo, object obj, object value)
    {
        if (memberInfo is FieldInfo field)
        {
            field.SetValue(obj, value);
        }
        else if (memberInfo is PropertyInfo property)
        {
            property.SetValue(obj, value);
        }
    }

    public static bool IsReadonlyProperty(this MemberInfo member)
    {
        return member is PropertyInfo property && !property.CanWrite;
    }
}
