using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Eviscerate() : ShadowSilentCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new RepeatVar(3),
        new EnergyVar(1),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
    ];
    
    private bool HasDiscardedThisTurn =>
        CombatManager.Instance.History.Entries
            .OfType<CardDiscardedEntry>()
            .Any(e => e.HappenedThisTurn(CombatState) && e.Card.Owner == Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override async Task AfterCardDiscarded(
        PlayerChoiceContext choiceContext,
        CardModel card)
    {
        if (CombatState == null || card.Owner != Owner)
            return;
        ReduceCostBy(DynamicVars.Energy.IntValue);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || this.IsClone )
            return Task.CompletedTask;
        ReduceCostBy(CombatManager.Instance.History.Entries.OfType<CardDiscardedEntry>().Count(e => e.HappenedThisTurn(CombatState) && e.Card.Owner == Owner));
        return Task.CompletedTask;
    }
    private void ReduceCostBy(int amount) => this.EnergyCost.AddThisTurn(-amount);

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
