using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class RoyalCloak() : ShadowRegentCard(2,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(1, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RoyalCloakPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Block.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class RoyalCloakPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Player == null) return;

        await CreatureCmd.GainBlock(Owner,
            PileType.Hand.GetPile(Owner.Player).Cards.Count * Amount, ValueProp.Unpowered,
            null);
    }
}