using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Hooks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Pilfer() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cargoPile = CargoCardPile.CargoPileType.GetPile(Owner);
        if (!cargoPile.IsEmpty)
        {
            var cardModels = cargoPile.Cards.Take((int)DynamicVars.Cards.BaseValue).ToList();
            foreach (var cardModel in cardModels)
            {
                await CardPileCmd.Add(cardModel, PileType.Hand);
                if (Owner.Creature.CombatState == null) continue;
                await Hook.AfterCardDrawn(Owner.Creature.CombatState, choiceContext, cardModel, true);
            }
        }
        
       }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}