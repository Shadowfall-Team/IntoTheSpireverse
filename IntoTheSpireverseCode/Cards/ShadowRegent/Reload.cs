using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Variables;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Reload() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(1),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("AttacksPlayed").WithMultiplier((card, _) =>
            CombatManager.Instance.History.CardPlaysFinished.Count(cardPlayEntry =>
                cardPlayEntry.HappenedThisTurn(card.CombatState) &&
                cardPlayEntry.CardPlay.Card.Type == CardType.Attack && cardPlayEntry.CardPlay.Card.Owner == card.Owner))
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        var ammoToLoad = (int)DynamicVars[LoadAmmoVar.Key].BaseValue +
                         ((CalculatedVar)DynamicVars["AttacksPlayed"]).Calculate(cardPlay.Target);
        await LoadAmmoCmd.LoadAmmo(ammoToLoad, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}