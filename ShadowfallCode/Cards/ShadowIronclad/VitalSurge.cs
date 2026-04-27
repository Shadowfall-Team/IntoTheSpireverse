using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class VitalSurge() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string CalculatedHealKey = "CalculatedHeal";

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedHealKey).WithMultiplier((card, _) => GetHpLostThisTurn(card)),
    ];

    private static decimal GetHpLostThisTurn(CardModel card)
    {
        if (card.Owner?.Creature == null || card.CombatState == null) return 0m;
        return CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Where(e => e.HappenedThisTurn(card.CombatState)
                        && e.Receiver == card.Owner.Creature)
            .Sum(e => (decimal)e.Result.UnblockedDamage);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        decimal heal = ((CalculatedVar)DynamicVars[CalculatedHealKey]).Calculate(null);
        if (heal > 0)
            await CreatureCmd.Heal(Owner.Creature, (int)heal);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}