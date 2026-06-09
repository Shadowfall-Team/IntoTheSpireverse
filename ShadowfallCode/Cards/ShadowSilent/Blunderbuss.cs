using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Blunderbuss() : ShadowSilentCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const string CalculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CalculationBaseVar(1m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedHitsKey).WithMultiplier((card, _) =>
            (decimal)PileType.Hand.GetPile(card.Owner).Cards
                .Count(c => c != card && GetEffectiveCost(c, card.Owner) >= 3)),
    ];
    
    private static int GetEffectiveCost(CardModel card, Player owner)
    {
        if (card.EnergyCost.CostsX)
            return owner.PlayerCombatState?.Energy ?? 0;
        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energyBeforePlay = (Owner.PlayerCombatState?.Energy ?? 0)
                               + cardPlay.Resources.EnergySpent;

        int hits = 1 + PileType.Hand.GetPile(Owner).Cards
            .Count(c =>
            {
                if (c.EnergyCost.CostsX)
                    return energyBeforePlay >= 3;
                return c.EnergyCost.GetWithModifiers(CostModifiers.All) >= 3;
            });

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() =>
        DynamicVars.Damage.UpgradeValueBy(3m);
}