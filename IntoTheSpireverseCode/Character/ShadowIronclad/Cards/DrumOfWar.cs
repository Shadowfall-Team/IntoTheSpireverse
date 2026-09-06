using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>The Transform half of the refund is settled by TransformPayoutPatches, not inline.</summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class DrumOfWar() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self),
    ITransformPayout
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new CardsVar(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this)
            await DrawPayout(choiceContext);
    }

    public Task OnTransformedAway(PlayerChoiceContext choiceContext) => DrawPayout(choiceContext);

    /// <summary>
    /// Guards on the owner rather than CombatState: that property is derived from the card's current
    /// pile, and a Transformed card has already left its pile by the time the payout settles, so it
    /// reads null and the draw would be skipped. Exhaust keeps the card in a combat pile, which is
    /// why only the Transform half was affected.
    /// </summary>
    private async Task DrawPayout(PlayerChoiceContext choiceContext)
    {
        var owner = Owner;
        if (owner?.Creature.CombatState == null) return;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
