/*
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Penance() : ShadowNecrobinderCard(-1, CardType.Curse, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SoulStrike>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
        IntoTheSpireverseKeywords.Startup
    ];
    
    public override int MaxUpgradeLevel => 0;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        
        // A different hook may be needed if it should be possible to draw a Soul Strike in the opening hand, currently
        // this happens AFTER the opening hand is drawn
        
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1) return;
        var soulStrikes = SoulStrike.Create(Owner, DynamicVars.Cards.IntValue, combatState);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(soulStrikes, PileType.Draw, Owner, CardPilePosition.Random));
    }
}
*/
