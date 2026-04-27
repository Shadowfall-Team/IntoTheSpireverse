using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

//TODO: Check what the upgrade is here
public class TrialOfSpace() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<TrialOfSpacePower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
    }
}

public class TrialOfSpacePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("CardsPlayedThisTurn", 0)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress)
            {
                DynamicVars["CardsPlayedThisTurn"].BaseValue++;
                if (DynamicVars["CardsPlayedThisTurn"].BaseValue % 14 == 0)
                {
                    StartPulsing();
                }

                if (DynamicVars["CardsPlayedThisTurn"].BaseValue % 15 == 0)
                {
                    Flash();

                    if (Owner.Player?.PlayerCombatState?.AllCards == null) return;
                    foreach (var cardModel in Owner.Player.PlayerCombatState.AllCards)
                    {
                        if (cardModel.IsUpgradable)
                        {
                            CardCmd.Upgrade(cardModel);
                        }
                    }

                    await PowerCmd.Remove(this);
                }
            }
        }
    }
}