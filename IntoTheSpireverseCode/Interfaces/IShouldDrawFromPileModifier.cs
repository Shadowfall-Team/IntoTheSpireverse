using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Interfaces;

public interface IShouldDrawFromPileModifier
{
    /// <summary>
    /// Similar usage to <see cref="Hook.ShouldDraw(ICombatState, Player, bool, out AbstractModel?)"/>,
    /// though doesn't inherently call that hook and propagate it's result
    /// </summary>
    /// <param name="pileType"><see cref="PileType"/> being drawn from.</param>
    /// <param name="player">Player drawing cards.</param>
    /// <param name="startOfTurnDraw">Is the instance of draw is part of the <paramref name="player"/>'s turn setup.</param>
    /// <returns>Whether the player is allowed to draw cards</returns>
    public bool ShouldDrawFromPile(PileType pileType, Player player, bool startOfTurnDraw);
}
