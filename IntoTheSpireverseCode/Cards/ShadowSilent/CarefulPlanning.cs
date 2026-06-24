using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

public sealed class CarefulPlanning() : ShadowSilentCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Nimble", 3m),
        new StringVar("Enchantment", ModelDb.Enchantment<Nimble>().Title.GetFormattedText()),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
    }

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card != this) return Task.CompletedTask;

        decimal nimbleAmount = DynamicVars["Nimble"].BaseValue;
        var nimble = ModelDb.Enchantment<Nimble>();

        foreach (var handCard in PileType.Hand.GetPile(Owner).Cards.ToList())
        {
            if (!nimble.CanEnchant(handCard)) continue;

            CardCmd.Enchant<Nimble>(handCard, nimbleAmount);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Nimble"].UpgradeValueBy(1m);
    }
}
