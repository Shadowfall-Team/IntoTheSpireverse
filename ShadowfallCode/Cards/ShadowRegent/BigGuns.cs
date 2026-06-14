using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Commands;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;
using Shadowfall.ShadowfallCode.utils;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class BigGuns() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("BigGuns", 2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => LoadAmmoHoverTip.FromLoadAmmo();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<BigGunsPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars["BigGuns"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BigGuns"].UpgradeValueBy(1);
    }
}

public class BigGunsPower : CustomPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public string GetSecondAmount()
    {
        return (DynamicVars["EnergySpent"].BaseValue % 10).ToString();
    }


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("EnergySpent", 0)
    ];

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress && amount > 0)
            {
                DynamicVars["EnergySpent"].BaseValue += amount;
                InvokeDisplayAmountChanged();
                if (DynamicVars["EnergySpent"].BaseValue % 9 == 0)
                {
                    StartPulsing();
                }

                if (DynamicVars["EnergySpent"].BaseValue > 9)
                {
                    Flash();

                    await LoadAmmoCmd.LoadAmmo(Amount, Owner.Player, this);

                    DynamicVars["EnergySpent"].BaseValue -= 10;
                    StopPulsing();
                }
            }
        }
    }

}