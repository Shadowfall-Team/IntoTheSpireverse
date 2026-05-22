using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class EscortMe() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<MinionStrike>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        var tripCard = CombatState.CreateCard<MinionStrike>(Owner);
        await CardPileCmd.Add(tripCard, PileType.Hand, clonedBy: this);
        
        var xValue = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            xValue += 1;
        }
        
        await CardPileCmd.AddToCombatAndPreview<MinionStrike>(
            Owner.Creature,
            CargoCardPile.CargoPileType, xValue, Owner);
        
    }
}