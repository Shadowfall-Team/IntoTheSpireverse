using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class RedGiant() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;
    //TODO: Not sure if extra is needed for multiplayer. Plz playtest
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.SingleplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
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