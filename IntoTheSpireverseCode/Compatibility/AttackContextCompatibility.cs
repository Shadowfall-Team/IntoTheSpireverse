using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class AttackContextCompatibility
{

    private static MethodInfo? _mainMethod;
    private static MethodInfo? MainMethod
    {
        get
        {
            _mainMethod ??= AccessTools.Method(typeof(AttackCommand), nameof(AttackCommand.CreateContextAsync),
                [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel)]);
            return _mainMethod;
        }
    }
    private static MethodInfo? _betaMethod;
    private static MethodInfo? BetaMethod
    {
        get
        {
            _betaMethod ??= AccessTools.Method(typeof(AttackCommand), nameof(AttackCommand.CreateContextAsync),
                [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardPlay)]);
            return _betaMethod;
        }
    }

    /// <summary>
    /// Compatibility wrapper for main/beta branch methods.
    /// Third param <paramref name="cardPlay"/> is <see cref="CardModel"/> cardSource on main branch,
    /// which can be derived from the <paramref name="cardPlay"/> param used in the beta version
    /// </summary>
    public static async Task<AttackContext> CreateContextAsync(ICombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (MainMethod != null)
        {
            return await (Task<AttackContext>)MainMethod.Invoke(null, [combatState, choiceContext, cardPlay.Card]);
        }

        if (BetaMethod != null)
        {
            return await (Task<AttackContext>)BetaMethod.Invoke(null, [combatState, choiceContext, cardPlay]);
        }

        throw new MissingMethodException("AttackCommand.CreateContextAsync overload not recognised");
    }
}
