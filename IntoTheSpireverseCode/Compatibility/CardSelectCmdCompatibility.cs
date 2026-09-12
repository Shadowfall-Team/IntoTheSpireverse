using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CardSelectCmdCompatibility
{
    public static IDisposable? PushSelectorCompatibility(ICardSelector selector, bool isLocalOnly = false)
    {
        var mainSelectorMethod = AccessTools.Method(typeof(CardSelectCmd), nameof(CardSelectCmd.PushSelector), parameters: [typeof(ICardSelector)]);
        var betaSelectorMethod = AccessTools.Method(typeof(CardSelectCmd), nameof(CardSelectCmd.PushSelector), parameters: [typeof(ICardSelector), typeof(bool)]);
        if ((mainSelectorMethod ?? betaSelectorMethod) is null)
        {
            MainFile.Logger.Error("Method Signiature for CardSelectCmd.PushSelector not recognised");
            return null;
        }
        if (mainSelectorMethod is not null)
        {
            return mainSelectorMethod.Invoke(null, [selector]) as IDisposable;
        }
        else
        {
            return betaSelectorMethod.Invoke(null, [selector, isLocalOnly]) as IDisposable;
        }
    }
}


