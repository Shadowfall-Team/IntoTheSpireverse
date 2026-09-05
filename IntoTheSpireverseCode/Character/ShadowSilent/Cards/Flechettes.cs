using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Flechettes() : ShadowSilentCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string _calculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar(_calculatedHitsKey).WithMultiplier((card, _) => PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Skill))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
	    if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int) ((CalculatedVar) DynamicVars[_calculatedHitsKey]).Calculate(cardPlay.Target))
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath, tmpSfx: "dagger_throw.mp3")
            .Execute(choiceContext);
	}

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
