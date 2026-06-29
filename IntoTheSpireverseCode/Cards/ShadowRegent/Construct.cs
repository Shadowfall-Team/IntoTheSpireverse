using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Construct() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShardsPower>(),
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromCard<Hyperdrive>(IsUpgraded)

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        var driveCard = CombatState.CreateCard<Hyperdrive>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(driveCard);
        }
        var cardAdd = await CardPileCmd.AddGeneratedCardToCombat(driveCard, CargoCardPile.CargoPileType, Owner);
        CardCmd.PreviewCardPileAdd(cardAdd);
    }

    protected override void OnUpgrade()
    {
        
    }
}
