using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TargetAcquired() : ShadowRegentCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1M, ValueProp.Move),
        new IntVar("LoadAmmo", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        await PowerCmd.Apply<TargetedPower>(
            new ThrowingPlayerChoiceContext(),
            play.Target,
            1,
            Owner.Creature,
            this);

        await LoadAmmoCmd.LoadAmmo(DynamicVars["LoadAmmo"].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}