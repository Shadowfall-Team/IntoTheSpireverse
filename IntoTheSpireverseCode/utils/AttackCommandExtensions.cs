using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse;

public static class AttackCommandExtensions
{
    
    // Class exists so Incite Violence's recoil damage only targets the attacker and not ALL players
    
    private static readonly FieldInfo SourceTypeField =
        typeof(AttackCommand).GetField("_sourceType", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly PropertyInfo AttackerProperty =
        typeof(AttackCommand).GetProperty("Attacker");

    public static AttackCommand FromMonsterSingleTarget(this AttackCommand command, MonsterModel monster)
    {
        AttackerProperty.SetValue(command, monster.Creature);
        SourceTypeField.SetValue(command, 1);
        return command;
    }
}