using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;

public sealed class BattleShoutModification : Modification, IModifyDamageAdditive
{
    protected override ModelId SourceCardId => ModelDb.Card<BattleShout>().Id;

    // The printed damage already includes the bonus; appended text would read as a second one.
    protected override bool AppendsTextToCardDescription => false;

    // The game's damage hook, since BaseLib marks CardModifier.ModifyBaseDamageAdditive unfinished.
    // cardSource is set for previews too, so the printed damage includes the bonus.
    public decimal ModifyDamageAdditiveCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay) =>
        cardSource == Owner && !props.HasFlag(ValueProp.Unpowered) ? Amount : 0m;
}
