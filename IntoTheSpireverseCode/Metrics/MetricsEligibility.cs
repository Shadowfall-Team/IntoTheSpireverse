using System.Reflection;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Metrics;

internal static class MetricsEligibility
{
    public static bool IsEligible(SerializableRun run, ulong localPlayerId)
    {
        if (RunManager.Instance.IsAbandoned)
        {
            return false;
        }

        if (AutoSlayer.IsActive)
        {
            return false;
        }

        if (run.GameMode != GameMode.Standard)
        {
            return false;
        }

        if (run.MapPointHistory.SelectMany(logs => logs).Count() < 5)
        {
            return false;
        }

        if (run.Ascension < 0)
        {
            return false;
        }

        var localPlayer = run.Players.FirstOrDefault(p => p.NetId == localPlayerId);
        if (localPlayer?.CharacterId is not { } localCharacterId)
        {
            return false;
        }

        var localCharacterModel = ModelDb.GetByIdOrNull<CharacterModel>(localCharacterId);
        if (localCharacterModel is null || localCharacterModel.GetType().Assembly != typeof(MainFile).Assembly)
        {
            return false;
        }

        return !HasForeignContent(run);
    }

    private static bool HasForeignContent(SerializableRun run)
    {
        if (run.Acts.Any(act => !IsAllowed<ActModel>(act.Id)))
        {
            return true;
        }

        foreach (var player in run.Players)
        {
            if (!IsAllowed<CharacterModel>(player.CharacterId))
            {
                return true;
            }

            if (player.Deck.Any(card => !IsAllowed<CardModel>(card.Id)))
            {
                return true;
            }

            if (player.Relics.Any(relic => !IsAllowed<RelicModel>(relic.Id)))
            {
                return true;
            }

            if (player.Potions.Any(potion => !IsAllowed<PotionModel>(potion.Id)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsAllowed<T>(ModelId? id) where T : AbstractModel
    {
        if (id is null || id == ModelId.none)
        {
            return true;
        }

        var model = ModelDb.GetByIdOrNull<T>(id);
        return model is not null && IsAllowedAssembly(model.GetType().Assembly);
    }

    private static bool IsAllowedAssembly(Assembly assembly)
    {
        return assembly == typeof(MainFile).Assembly || assembly == typeof(CharacterModel).Assembly;
    }
}