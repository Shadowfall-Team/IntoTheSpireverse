using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class SpiritualWeapon() : ShadowSilentCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
    IntoTheSpireverseKeywords.IMuddleListener
{
    private const string IncreaseKey = "Increase";
    private decimal _extraDamage;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar(IncreaseKey, 10m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public void OnMuddled()
    {
        decimal increase = DynamicVars[IncreaseKey].BaseValue;
        DynamicVars.Damage.BaseValue += increase;
        _extraDamage += increase;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[IncreaseKey].UpgradeValueBy(4m);
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += _extraDamage;
    }
}
