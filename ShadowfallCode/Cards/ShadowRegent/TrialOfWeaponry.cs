using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class TrialOfWeaponry() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AmmoPower>(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmmoPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<TrialOfWeaponryPower>(Owner.Creature,
            DynamicVars[nameof(AmmoPower)].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(AmmoPower)].UpgradeValueBy(1);
    }
}

public class TrialOfWeaponryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("AttacksPlayedThisTurn", 0)
    ];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext,
        CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars["AttacksPlayedThisTurn"].BaseValue = 0;
        StopPulsing();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress)
            {
                if (cardPlay.Card.Type == CardType.Attack)
                {
                    DynamicVars["AttacksPlayedThisTurn"].BaseValue++;
                    if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 2 == 0)
                    {
                        StartPulsing();
                    }

                    if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 3 == 0)
                    {
                        Flash();
                        await PowerCmd.Apply<AmmoPower>(Owner, Amount, Owner, null);
                        await PowerCmd.Remove(this);
                    }
                }
            }
        }
    }
}