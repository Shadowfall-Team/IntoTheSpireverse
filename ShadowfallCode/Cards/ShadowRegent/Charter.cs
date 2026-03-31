using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Charter() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await CardPileCmd.AddToCombatAndPreview<MinionStrike>(Owner.Creature,
            CargoCardPile.CargoPileType, DynamicVars.Cards.IntValue, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}