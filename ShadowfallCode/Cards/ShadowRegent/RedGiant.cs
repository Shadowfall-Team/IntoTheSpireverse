using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Rewards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class RedGiant() : ShadowRegentCard(
    1,
    //TODO: check if this should indeed be a power
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    //TODO: Not sure if extra is needed for multiplayer. Plz playtest
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.SingleplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<RedGiantPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
                1,
                Owner.Creature,
                this);
        }
        else
        {
            await PowerCmd.Apply<RedGiantRandomPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
                1,
                Owner.Creature,
                this);
        }
    }
}

public class RedGiantPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return Task.CompletedTask;
        // put them in one reward to reduce information overload?
        room.AddExtraReward(Owner.Player, new CardUpgradeReward(Owner.Player) {
            Amount = Amount
            });

        return Task.CompletedTask;
    }
}

public class RedGiantRandomPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return Task.CompletedTask;
        // random should be individual maybe?
        // in-case you only want to upgade a certain amount of cards?
        for (int i = 0; i < Amount; i++)
        {
            room.AddExtraReward(Owner.Player, new RandomCardUpgradeReward(Owner.Player));
        }
        return Task.CompletedTask;
    }
}
