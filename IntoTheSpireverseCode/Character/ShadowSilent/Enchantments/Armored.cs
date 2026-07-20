using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Enchantments;

public sealed class Armored : IntoTheSpireverseEnchantment
{
    public override bool CanEnchant(CardModel card) => base.CanEnchant(card) && card.GainsBlock;

    public override bool HasExtraCardText => true;

    public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3m, ValueProp.Move),
    ];
    
    public override Decimal EnchantBlockAdditive(Decimal originalBlock) => DynamicVars.Block.BaseValue;
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await PlayerCmd.GainEnergy(Amount, Card.Owner);
    }
}