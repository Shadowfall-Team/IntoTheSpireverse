using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

public class CargoDrawManager() : CustomSingletonModel(HookType.Combat)
{
    const int BaseDrawCount = 1;
    // BeforeHandDraw rather than AfterPlayerTurnStart: CombatManager runs this hook, then the
    // 5-card hand draw, then AfterPlayerTurnStart - so this is what puts Cargo cards in hand
    // ahead of the normal draw.
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        var drawAmount = Hook.ModifyStartOfTurnDrawFromPile(combatState, player, CargoCardPile.CargoPileType, BaseDrawCount, out var modifiers);
        await Hook.AfterModifyingStartOfTurnDrawFromPile(combatState, modifiers);
        await CardPileCmd.DrawFromPile(choiceContext, CargoCardPile.CargoPileType, drawAmount, player, true);
    }
}