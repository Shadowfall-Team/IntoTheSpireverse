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

    // Guards on Owner, not CombatState: a Transformed card has left its pile, so CombatState is null.
    private async Task DrawPayout(PlayerChoiceContext choiceContext)
    {
        var owner = Owner;
        if (owner?.Creature.CombatState == null) return;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
