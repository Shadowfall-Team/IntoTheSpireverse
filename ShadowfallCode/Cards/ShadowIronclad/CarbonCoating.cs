using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class CarbonCoating() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string CalculatedSlateKey = "CalculatedSlate";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedSlateKey)
            .WithMultiplier((card, _) => CombatManager.Instance.History.CardPlaysFinished
                .Count(e => e.HappenedThisTurn(card.CombatState)
                            && e.CardPlay.Card.Type == CardType.Attack
                            && e.CardPlay.Card.Owner == card.Owner)),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlatePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var amount = ((CalculatedVar)DynamicVars[CalculatedSlateKey]).Calculate(Owner.Creature);
        if (amount > 0)
        {
            await PowerCmd.Apply<SlatePower>(
                Owner.Creature, amount,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}