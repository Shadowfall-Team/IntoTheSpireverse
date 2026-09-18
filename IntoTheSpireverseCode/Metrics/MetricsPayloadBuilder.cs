using System.Text.Json;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Metrics;

internal static class MetricsPayloadBuilder
{

    public static RunMetrics BuildRunMetrics(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        LocManager.Instance.StartOverridingLanguageAsEnglish();
        try
        {
            return BuildRunMetricsCore(run, isVictory, localPlayerId);
        }
        finally
        {
            LocManager.Instance.StopOverridingLanguageAsEnglish();
        }
    }

    private static RunMetrics BuildRunMetricsCore(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        var history = run.MapPointHistory.SelectMany(logs => logs).ToList();

        var killedByEncounter = ModelId.none;
        var lastEntry = run.MapPointHistory.LastOrDefault()?.LastOrDefault();
        if (!isVictory && lastEntry is not null && lastEntry.Rooms.Last().RoomType.IsCombatRoom())
        {
            killedByEncounter = lastEntry.Rooms.Last().ModelId!;
        }

        var localPlayer = run.Players.First(p => p.NetId == localPlayerId);

        var encounters = history
            .Where(e => e.Rooms.Last().RoomType.IsCombatRoom())
            .Select(e => new EncounterMetric(
                e.Rooms.Last().ModelId!.Entry,
                Math.Min(e.GetEntry(localPlayerId).DamageTaken, localPlayer.MaxHp),
                e.Rooms.Last().TurnsTaken + 1))
            .ToList();

        var cardChoices = history
            .Where(e => e.GetEntry(localPlayerId).CardChoices.Count > 0)
            .Select(e => new CardChoiceMetric(e.GetEntry(localPlayerId).CardChoices))
            .ToList();

        var ancientChoices = history
            .Where(e => e.MapPointType == MapPointType.Ancient)
            .Where(e => e.GetEntry(localPlayerId).AncientChoices.Count > 0)
            .Select(e => new AncientMetric(e, e.GetEntry(localPlayerId)))
            .ToList();

        var actWins = new List<ActWinMetric>();
        var eventChoices = new List<EventChoiceMetric>();
        for (var actIndex = 0; actIndex < run.MapPointHistory.Count; actIndex++)
        {
            foreach (var entry in run.MapPointHistory[actIndex])
            {
                if (entry.Rooms.First().RoomType == RoomType.Event
                    && entry.GetEntry(localPlayerId).EventChoices.Count != 0
                    && entry.MapPointType != MapPointType.Ancient)
                {
                    eventChoices.Add(new EventChoiceMetric(entry, localPlayerId, run.Acts[actIndex]));
                }
            }

            var actWin = actIndex < run.MapPointHistory.Count - 1 || isVictory;
            actWins.Add(new ActWinMetric(run.Acts[actIndex].Id!.Entry, actWin));
        }

        var progress = SaveManager.Instance.Progress;

        return new RunMetrics
        {
            Ascension = run.Ascension,
            TotalPlaytime = progress.TotalPlaytime,
            TotalWinRate = (float)progress.Wins / progress.NumberOfRuns,
            NumReloads = run.NumReloads,
            BuildId = ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? "NON-RELEASE-VERSION",
            BuildType = PlatformUtil.GetPlatformBranch().ToName(),
            PlayerId = progress.UniqueId,
            Character = localPlayer.CharacterId!,
            NumPlayers = run.Players.Count,
            Team = run.Players.Count > 1 ? run.Players.Select(p => p.CharacterId!).ToList() : [],
            Win = isVictory,
            FloorReached = history.Count,
            KilledByEncounter = killedByEncounter,
            Deck = localPlayer.Deck.Select(c => c.Id!),
            Relics = localPlayer.Relics.Select(r => r.Id!),
            RunPlaytime = run.WinTime > 0 ? run.WinTime : run.RunTime,
            Encounters = encounters,
            CardChoices = cardChoices,
            EventChoices = eventChoices,
            AncientChoices = ancientChoices,
            ActWins = actWins,
            CampfireUpgrades = history
                .Where(e => e.MapPointType == MapPointType.RestSite)
                .SelectMany(e => e.GetEntry(localPlayerId).UpgradedCards)
                .Select(c => c.Entry)
                .ToList(),
            RelicBuys = history.SelectMany(e => e.GetEntry(localPlayerId).BoughtRelics).Select(r => r.Entry).ToList(),
            PotionBuys = history.SelectMany(e => e.GetEntry(localPlayerId).BoughtPotions).Select(p => p.Entry).ToList(),
            ColorlessBuys = history.SelectMany(e => e.GetEntry(localPlayerId).BoughtColorless).Select(c => c.Entry).ToList(),
            PotionDiscards = history.SelectMany(e => e.GetEntry(localPlayerId).PotionDiscarded).Select(p => p.Entry).ToList(),
        };
    }
    
    public static string BuildPayloadJson(RunMetrics metrics)
    {
        var runMetrics = JsonSerializer.Serialize(metrics, MetricsPayloadSerializerContext.Default.RunMetrics);
        using var runMetricsJson = JsonDocument.Parse(runMetrics);

        var envelope = new MetricsPayload
        {
            SchemaVersion = 1,
            Environment = MetricsConfig.Environment,
            ModVersion = ResolveModVersion(),
            Data = runMetricsJson.RootElement.Clone(),
        };

        return JsonSerializer.Serialize(envelope, MetricsPayloadSerializerContext.Default.MetricsPayload);
    }

    private static string ResolveModVersion()
    {
        foreach (var mod in ModManager.GetLoadedMods())
        {
            if (mod.manifest?.id == MainFile.ModId)
            {
                return mod.manifest.version ?? "unknown";
            }
        }

        return "unknown";
    }
}
