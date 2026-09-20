using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class AttackContextCompatibility
{
    /// <summary>
    /// Compatibility wrapper for main/beta branch methods.
    /// Third param <paramref name="cardPlay"/> is <see cref="CardModel"/> cardSource on main branch,
    /// which can be derived from the <paramref name="cardPlay"/> param used in the beta version
    /// </summary>
    public static async Task<AttackContext> CreateContextAsync(ICombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mainMethod = AccessTools.Method(typeof(AttackCommand), nameof(AttackCommand.CreateContextAsync),
            [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel)]);
        var betaMethod = AccessTools.Method(typeof(AttackCommand), nameof(AttackCommand.CreateContextAsync),
            [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardPlay)]);

        if (mainMethod != null)
        {
            return await (Task<AttackContext>)mainMethod.Invoke(null, [combatState, choiceContext, cardPlay.Card]);
        }

        if (betaMethod != null)
        {
            return await (Task<AttackContext>)betaMethod.Invoke(null, [combatState, choiceContext, cardPlay]);
        }

        throw new MissingMethodException("AttackCommand.CreateContextAsync overload not recognised");
    }
}


