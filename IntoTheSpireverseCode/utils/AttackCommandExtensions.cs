using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse;

public static class AttackCommandExtensions
{
    
    // Class exists so Incite Violence's recoil damage only targets the attacker and not ALL players
    
    private static readonly FieldInfo SourceTypeField =
        typeof(AttackCommand).GetField("_sourceType", BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingFieldException("AttackCommand._sourceType not found — game update changed internals.");

    private static readonly PropertyInfo AttackerProperty =
        typeof(AttackCommand).GetProperty("Attacker")
        ?? throw new MissingMemberException("AttackCommand.Attacker not found — game update changed internals.");

    public static AttackCommand FromMonsterSingleTarget(this AttackCommand command, MonsterModel monster)
    {
        AttackerProperty.SetValue(command, monster.Creature);
        SourceTypeField.SetValue(command, 1);
        return command;
    }
}