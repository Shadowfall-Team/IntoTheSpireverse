using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class SlatePower : ShadowPowerModel
{
    private const string BlockKey = "Block";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BlockKey, 4m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
    ];

    protected override object InitInternalData() => new Data();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        GetInternalData<Data>().AppliedBy = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var data = GetInternalData<Data>();
        if (data.AppliedBy == cardPlay.Card)
        {
            data.AppliedBy = null;
            return;
        }

        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card.Type != CardType.Attack)
            return;

        Flash();
        var riebeckite = Owner.Powers.OfType<RiebeckitePower>().FirstOrDefault();

        decimal blockAmount = DynamicVars[BlockKey].BaseValue + (riebeckite?.Amount ?? 0);
        await CreatureCmd.GainBlock(Owner, blockAmount, ValueProp.Unpowered, null);

        if (riebeckite != null)
            await PowerCmd.Apply<RetaliationPower>(
                new ThrowingPlayerChoiceContext(),
                Owner, riebeckite.DynamicVars[RiebeckitePower.RetaliationKey].BaseValue, Owner, null);

        await PowerCmd.Decrement(this);

        if (cardPlay.Card is ISlateSpender spender)
            await spender.OnSlateSpent(choiceContext, cardPlay);
    }

    private class Data
    {
        public CardModel? AppliedBy;
    }
}