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

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Shellback() : ShadowSilentCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string EnergyKey = "EnergyCost";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShellbackPower>(6m),
        new DynamicVar(EnergyKey, 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        var power = await PowerCmd.Apply<ShellbackPower>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<ShellbackPower>().BaseValue,
            Owner.Creature, this);

        if (power != null)
            power.AddCost(DynamicVars[EnergyKey].IntValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Power<ShellbackPower>().UpgradeValueBy(2m);
    }
}