using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Avarice() : ShadowNecrobinderCard(-1, CardType.Curse, CardRarity.Uncommon, TargetType.None)
{
    private const string _goldKey = "Gold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(_goldKey, 10m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
        ShadowfallKeywords.Startup
    ];

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1) return;
        await PlayerCmd.GainGold(DynamicVars[_goldKey].IntValue, Owner);
    }
}