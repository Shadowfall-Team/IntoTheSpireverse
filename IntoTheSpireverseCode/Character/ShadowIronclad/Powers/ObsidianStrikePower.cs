using IntoTheSpireverse.IntoTheSpireverseCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class ObsidianStrikePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static async Task<int> AdjustPlayCount(Task<int> basePlayCountTask, CardModel card,
        Creature? target, bool isAutoPlay)
    {
        var playCount = await basePlayCountTask;
        if (target == null || isAutoPlay ||
            (IndirectPlayTracker.TryGetLastPileLeft(card, out var pile) && pile != PileType.Hand))
        {
            return playCount;
        }

        var power = target.Powers.OfType<ObsidianStrikePower>().FirstOrDefault();
        if (power == null)
        {
            return playCount;
        }

        power.Flash();
        await PowerCmd.Decrement(power);
        return playCount + 1;
    }
}