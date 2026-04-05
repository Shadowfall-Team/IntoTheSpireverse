using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Armada() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await PowerCmd.Apply<AmmoPower>(Owner.Creature,
                1,
                Owner.Creature,
                this);
        }

        await PowerCmd.Apply<ArmadaPower>(Owner.Creature,
            1,
            Owner.Creature,
            this);
    }
}

public class ArmadaPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterSideTurnStart(CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Side || AmountOnTurnStart == 0)
            return;

        await PowerCmd.Apply<AmmoPower>(Owner, Amount, Owner, null);
    }
}