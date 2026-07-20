using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Cards;

public sealed class AbraCadaver() : ShadowNecrobinderCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string _calculatedStrKey = "CalculatedStr";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(_calculatedStrKey).WithMultiplier(static (card, _) =>
            card.Owner.PlayerCombatState?.AllCards.Count(c => c.Type == CardType.Curse) ?? 0),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        Decimal amount = ((CalculatedVar)DynamicVars[_calculatedStrKey]).Calculate(null);
        await PowerCmd.Apply<AbraCadaverPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }
}