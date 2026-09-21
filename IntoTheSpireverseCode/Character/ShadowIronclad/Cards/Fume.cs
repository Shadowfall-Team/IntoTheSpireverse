using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Fume() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Shuffle first so the peek sees the card AutoPlayFromDrawPile will actually take: with an
        // empty Draw Pile it shuffles the Discard back in before picking, and peeking ahead of that
        // would read nothing and miss the Unplayable case.
        await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);

        var top = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault();
        var wasUnplayable = top != null && top.Keywords.Contains(CardKeyword.Unplayable);

        await CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner, 1, CardPilePosition.Top, forceExhaust: false);

        // AutoPlay moves an Unplayable card to its result pile without playing it, so the card is
        // consumed either way and the Strength stands in for the effect it would have had.
        if (!wasUnplayable) return;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature, DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
