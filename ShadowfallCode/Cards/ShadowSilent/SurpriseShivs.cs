using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class SurpriseShivs() : ShadowSilentCard(3, CardType.Power, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar("Shivs", 2),
        new PowerVar<AccuracyPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Devious),
        HoverTipFactory.FromCard<Shiv>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShadowfallKeywords.ExecuteDevious(choiceContext, Owner, this, async () =>
        {
            await Shiv.CreateInHand(Owner, DynamicVars["Shivs"].IntValue, CombatState);
            await PowerCmd.Apply<AccuracyPower>(Owner.Creature, DynamicVars[nameof(AccuracyPower)].BaseValue, Owner.Creature, this);
        });
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
