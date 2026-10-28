using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class LoadTheCannon() : ShadowRegentCard(1,
    CardType.Attack,
    CardRarity.Basic,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoadAmmoVar(1),
        new DamageVar(3, ValueProp.Move),
        new SpireverseHitCountVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ..LoadAmmoHoverTip.FromLoadAmmo(),
        ..Artists.AnneBean.HoverTip(this)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.SpireverseHitCount.IntValue)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        await LoadAmmoCmd.LoadAmmo(DynamicVars.LoadAmmo.BaseValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.SpireverseHitCount.UpgradeValueBy(1m);
    }
}
