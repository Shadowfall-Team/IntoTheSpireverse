using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Obsidian Strike's mark. The next card that targets this creature is copied for free onto the top
/// of the marked player's Draw Pile.
///
/// The copy goes to the Draw Pile rather than the Hand deliberately. Obsidian Strike targets an
/// enemy itself, so a copy landing in Hand would be replayable immediately, re-arming the mark and
/// emitting another free copy every cycle at no cost. Routing through the Draw Pile gates the loop
/// behind card draw, which turns a degenerate engine into a draw-engine combo.
/// </summary>
public sealed class ObsidianStrikePower : ShadowPowerModel
{
    private class Data
    {
        /// <summary>The card that applied this mark, so its own play does not immediately consume it.</summary>
        public CardModel? AppliedBy;

        /// <summary>Whoever played Obsidian Strike always receives the copy, even in co-op where
        /// another player may be the one who triggers the mark.</summary>
        public Player? Beneficiary;
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Each source needs its own instance, because the instance is what remembers who receives the
    /// copy. Two players marking the same enemy must not collapse into one stack, or the second
    /// application would redirect the first player's copies. Repeat applications from the same
    /// player still stack onto that player's own instance, which is exactly what this mode does.
    /// </summary>
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override object InitInternalData() => new Data();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        data.AppliedBy = cardSource;
        data.Beneficiary = applier?.Player ?? cardSource?.Owner;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var data = GetInternalData<Data>();

        // The Obsidian Strike that applied this mark targets the enemy too; skip its own play.
        if (data.AppliedBy == cardPlay.Card)
        {
            data.AppliedBy = null;
            return;
        }

        // Explicit single-target only. Cards hitting ALL enemies carry no Target and do not count
        // as targeting this enemy.
        if (cardPlay.Target != Owner) return;

        var beneficiary = data.Beneficiary;
        if (beneficiary == null || Owner.CombatState == null) return;

        Flash();

        var copy = cardPlay.Card.CreateCloneForPlayer(beneficiary);
        copy.SetToFreeThisCombat();
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, beneficiary, CardPilePosition.Top);

        await PowerCmd.Decrement(this);
    }
}
