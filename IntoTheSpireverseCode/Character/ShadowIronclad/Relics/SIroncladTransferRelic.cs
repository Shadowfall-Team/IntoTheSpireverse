using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class Bellows : ShadowIroncladRelic
{
    private const string CardsKey = "Cards";

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(CardsKey, 2m),
    ];
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Owner.PlayerCombatState?.TurnNumber > 1) return;

        Flash();
        await CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner,
            DynamicVars[CardsKey].IntValue, CardPilePosition.Top, forceExhaust: false);
    }
}
