using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Blitz() : ShadowIroncladCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new SpireverseHitCountVar(HitsKey, 2),
        new SpireverseHitCountVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null || cardPlay.Target == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars[HitsKey].IntValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        // The chosen target is inside "ALL enemies" too, so it takes both sets of hits.
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.SpireverseHitCount.IntValue)
            .FromCardCompatibility(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.SpireverseHitCount.UpgradeValueBy(1m);
}
