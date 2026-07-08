using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class BlunderGuard() : ShadowSilentCard(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string StrengthKey = "StrengthGain";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlunderGuardPower>(6m),
        new DynamicVar(StrengthKey, 2m),
        new EnergyVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        var power = await PowerCmd.Apply<BlunderGuardPower>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<BlunderGuardPower>().BaseValue,
            Owner.Creature, this);

        if (power != null)
            power.AddStrength(DynamicVars[StrengthKey].IntValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Power<BlunderGuardPower>().UpgradeValueBy(2m);
        DynamicVars[StrengthKey].UpgradeValueBy(1m);
    }
}