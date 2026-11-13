using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CardSelectCmdCompatibility
{
    public static IDisposable? PushSelectorCompatibility(Player player, ICardSelector selector, bool isLocalOnly = false)
    {
        var v107SelectorMethod = AccessTools.Method(typeof(CardSelectCmd), nameof(CardSelectorStack.PushSelector), parameters: [typeof(ICardSelector)]);
        var v111SelectorMethod = AccessTools.Method(typeof(CardSelectCmd), nameof(CardSelectorStack.PushSelector), parameters: [typeof(ICardSelector), typeof(bool)]);
        var v113SelectorMethod = AccessTools.Method(typeof(CardSelectorStack), nameof(CardSelectorStack.PushSelector), parameters: [typeof(Player), typeof(ICardSelector)]);
        if ((v107SelectorMethod ?? v111SelectorMethod ?? v113SelectorMethod) is null)
        {
            MainFile.Logger.Error("Method Signiature for CardSelectCmd.PushSelector not recognised");
            return null;
        }

        if (v107SelectorMethod is not null)
        {
            return v107SelectorMethod.Invoke(null, [selector]) as IDisposable;
        }
        else if (v111SelectorMethod is not null)
        {
            return v111SelectorMethod.Invoke(null, [selector, isLocalOnly]) as IDisposable;
        }
        else
        {
            return v113SelectorMethod.Invoke(RunManager.Instance.CardSelectorStack, [player, selector]) as IDisposable;
        }
    }
}


