using BaseLib.Common.Rewards;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class RedGiant() : ShadowRegentCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<RedGiantRandomPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class RedGiantRandomPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return;
        for (int i = 0; i < Amount; i++)
        {
            room.AddExtraReward(Owner.Player, new RandomCardUpgradeReward(Owner.Player));
        }
    }
}