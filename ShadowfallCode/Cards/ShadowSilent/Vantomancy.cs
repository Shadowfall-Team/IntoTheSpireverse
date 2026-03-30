using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Vantomancy() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlipperyPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlipperyPower>(),
        HoverTipFactory.FromCard<Weight>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SlipperyPower>(Owner.Creature, DynamicVars[nameof(SlipperyPower)].BaseValue, Owner.Creature, this);

        for (int i = 0; i < 2; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Weight>(Owner), PileType.Hand, true);
        }
    }

    protected override void OnUpgrade() { 
        base.AddKeyword(CardKeyword.Retain);
    }
}
