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

    // The damage preview already folds this bonus into the card's printed number, so a line
    // saying "deals N additional damage" would read as a second bonus on top of it. The hover
    // tip still carries that text.
    protected override bool AppendsTextToCardDescription => false;

    /// <summary>
    /// The game's own damage hook rather than CardModifier.ModifyBaseDamageAdditive, which
    /// BaseLib marks "NOT YET FULLY FUNCTIONAL". Modifiers receive normal combat hooks
    /// (ShouldReceiveCombatHooks defers to the card they are attached to), so this works the
    /// same way PiercedPower does - keyed on cardSource being the card we are attached to.
    ///
    /// cardSource is set for previews as well as real plays, so the bonus shows in the card's
    /// displayed damage rather than only appearing on hit.
    /// </summary>
    public decimal ModifyDamageAdditiveCompability(
        Creature? target, 
        decimal amount, 
        ValueProp props, 
        Creature? dealer, 
        CardModel? cardSource, 
        CardPlay? cardPlay) =>
        cardSource == Owner ? Amount : 0m;
}
