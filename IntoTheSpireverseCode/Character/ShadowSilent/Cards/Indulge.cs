using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Indulge() : ShadowSilentCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(3m),
        new PowerVar<StrengthPower>(0m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        var power = await PowerCmd.Apply<PoisonPower>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<PoisonPower>().BaseValue,
            Owner.Creature, this);
      
        if (power != null)
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, Owner.Creature,
                power.Amount + DynamicVars.Power<StrengthPower>().BaseValue,
                Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
    }
}