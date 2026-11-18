using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CardSelectCmdCompatibility
{
    private readonly static MethodInfo? v111SelectorMethod;
    private readonly static MethodInfo? v113SelectorMethod;
    private readonly static MethodInfo? v113RunManagerCardSelectorStack;
    static CardSelectCmdCompatibility()
    {
        var cardSelectorStackType = AccessTools.TypeByName("CardSelectorStack");
        v111SelectorMethod = AccessTools.Method(typeof(CardSelectCmd), nameof(CardSelectorStack.PushSelector), parameters: [typeof(ICardSelector), typeof(bool)]);
        if (cardSelectorStackType is not null)
        {
            v113SelectorMethod = AccessTools.Method(cardSelectorStackType, nameof(CardSelectorStack.PushSelector), parameters: [typeof(Player), typeof(ICardSelector)]);
            v113RunManagerCardSelectorStack = AccessTools.PropertyGetter(typeof(RunManager), nameof(CardSelectorStack));
        }
    }

    public static IDisposable? PushSelectorCompatibility(Player player, ICardSelector selector, bool isLocalOnly = false)
    {
        if ((v111SelectorMethod ?? v113SelectorMethod) is null)
        {
            MainFile.Logger.Error("Method Signiature for CardSelectCmd.PushSelector not recognised");
            return null;
        }

        if (v111SelectorMethod is not null)
        {
            return v111SelectorMethod.Invoke(null, [selector, isLocalOnly]) as IDisposable;
        }
        else if (v113SelectorMethod is not null)
        {
            return v113SelectorMethod.Invoke(v113RunManagerCardSelectorStack?.Invoke(RunManager.Instance, []), [player, selector]) as IDisposable;
        }
        throw new InvalidOperationException();
    }
}
