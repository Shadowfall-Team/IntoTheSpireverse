using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class StrikeTrue() : ShadowSilentCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        
    [
        new PowerVar<VigorPower>(2m),
        new DynamicVar("Sharp", 4m),
        new StringVar("Enchantment", ModelDb.Enchantment<Sharp>().Title.GetFormattedText()),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(VigorPower)].BaseValue, Owner.Creature, this);
    }

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card != this) return Task.CompletedTask;

        decimal sharpAmount = DynamicVars["Sharp"].BaseValue;
        var sharp = ModelDb.Enchantment<Sharp>();

        foreach (var handCard in PileType.Hand.GetPile(Owner).Cards.ToList())
        {
            if (!sharp.CanEnchant(handCard)) continue;

            CardCmd.Enchant<Sharp>(handCard, sharpAmount);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(VigorPower)].UpgradeValueBy(1m);
        DynamicVars["Sharp"].UpgradeValueBy(2m);
    }
}
