using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards.Colorless;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Overdrive() : ShadowRegentCard(
    1,
    CardType.Attack,
    CardRarity.Ancient,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Warp>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("FakeBaseDamage", 4),
        new CalculationBaseVar(0),
        new ExtraDamageVar(4),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
        {
            var warpCount = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
                e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card is Warp);
            return (decimal)Math.Pow(2, warpCount);
        })
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FakeBaseDamage"].UpgradeValueBy(4);
        DynamicVars.ExtraDamage.UpgradeValueBy(4);
    }
}