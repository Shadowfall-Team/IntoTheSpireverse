using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

public class PurpleDough : ShadowRegentRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];


    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Creature.Side)
        {
            if (combatState.RoundNumber <= 1)
            {
                Flash();
                var cardModel = CardFactory.GetDistinctForCombat(Owner,
                    [
                        ModelDb.Card<SecretTechnique>(),
                        ModelDb.Card<SecretWeapon>(),
                        ModelDb.Card<MasterOfStrategy>()
                    ],
                    1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (cardModel != null)
                {
                    var cardPileAddResult = await CardPileCmd.AddGeneratedCardToCombat(cardModel, CargoCardPile.CargoPileType,
                        Owner);
                    CardCmd.PreviewCardPileAdd(cardPileAddResult);
                }
            }
        }
    }
}