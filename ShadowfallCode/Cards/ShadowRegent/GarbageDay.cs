using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class GarbageDay() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await CardPileCmd.AddToCombatAndPreview<Debris>(Owner.Creature,
            CargoCardPile.CargoPileType, 4, true);

        var bury = CombatState.CreateCard<Bury>(Owner);
        bury.EnergyCost.SetThisCombat(0);
        var cardPileAddResult = await CardPileCmd.Add(bury, CargoCardPile.CargoPileType);
        CardCmd.PreviewCardPileAdd(cardPileAddResult);
    }

    protected override void OnUpgrade()
    {
        //TODO What is the upgrade?
    }
}