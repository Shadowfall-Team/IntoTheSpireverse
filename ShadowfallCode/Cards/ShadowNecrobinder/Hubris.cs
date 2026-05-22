using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Hubris() : ShadowNecrobinderCard(-1, CardType.Curse, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
        ShadowfallKeywords.Startup
    ];
    
    public override int MaxUpgradeLevel => 0;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1) return;
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            combatState.HittableEnemies,
            DynamicVars.Damage,
            Owner.Creature);
    }
}