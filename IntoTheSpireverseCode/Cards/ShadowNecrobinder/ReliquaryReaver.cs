using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class ReliquaryReaver() : ShadowNecrobinderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        bool shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());

        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.Any(dr => dr.WasTargetKilled)))
            return;

        var targetRarity = IsUpgraded ? RelicRarity.Rare : RelicRarity.Uncommon;
        var relic = Owner.RelicGrabBag.PullFromFront(targetRarity, Owner.RunState);

        if (relic != null)
        {
            await RelicCmd.Obtain(relic.ToMutable(), Owner);
        }

        // Only remove if it's in the deck. Not really relevant except for dev purposes
        // since the only way you'll play the card in combat regularly is if it's in the deck at all
        if (DeckVersion != null && DeckVersion.Pile?.Type == PileType.Deck)
        {
            await CardPileCmd.RemoveFromDeck(DeckVersion);
        }
        await CardCmd.Exhaust(choiceContext, this);
    }

    protected override void OnUpgrade() { }
}
