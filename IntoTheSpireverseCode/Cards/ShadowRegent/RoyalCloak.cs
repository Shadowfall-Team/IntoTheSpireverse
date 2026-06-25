using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

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