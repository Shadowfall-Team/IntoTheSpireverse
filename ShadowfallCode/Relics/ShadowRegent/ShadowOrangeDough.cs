using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class ShadowOrangeDough : ShadowRegentRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override async Task AfterSideTurnStart(CombatSide side,
        CombatState combatState)
    {
        if (side == Owner.Creature.Side)
        {
            if (combatState.RoundNumber <= 1)
            {
                Flash();
                var list = CardFactory.GetDistinctForCombat(Owner,
                        ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(
                            Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint),
                        DynamicVars.Cards.IntValue,
                        Owner.RunState.Rng.CombatCardGeneration)
                    .ToList();
                Flash();
                var result = await CardPileCmd.AddGeneratedCardsToCombat(list, CargoCardPile.CargoPileType, true);
                CardCmd.PreviewCardPileAdd(result);
            }
        }
    }
}