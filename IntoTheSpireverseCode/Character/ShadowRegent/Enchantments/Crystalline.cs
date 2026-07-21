using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Enchantments;

public sealed class Crystalline : IntoTheSpireverseEnchantment
{
    public override bool HasExtraCardText => true;

    public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShardsPower>(1),
        new BlockVar(3m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShardsPower>(),
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await PowerCmd.Apply<ShardsPower>(new ThrowingPlayerChoiceContext(),
            Card.Owner.Creature,
            DynamicVars[nameof(ShardsPower)].BaseValue,
            Card.Owner.Creature,
            null);

        await CreatureCmd.GainBlock(Card.Owner.Creature, DynamicVars.Block, cardPlay);
    }
}
