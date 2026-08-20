using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

/// <summary>
/// Granted by the Mirror Mirror event. Card rewards start pulling from the player's mirror character's pool as well
/// as their own, and on pickup the player gains the mirror character's starting relic.
/// Rarity is Event, so this is never rolled as a normal relic reward (RelicFactory only rolls Common/Uncommon/Rare).
/// </summary>
[Pool(typeof(EventRelicPool))]
public class ParallelStone : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool HasUponPickupEffect => true;

    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }

    public override async Task AfterObtained()
    {
        if (Owner == null) return;

        var mirrorCharacter = AltCharacterUtil.GetMirrorCharacters(Owner.Character).FirstOrDefault();
        if (mirrorCharacter == null) return;

        var starter = AltCharacterUtil.GetStartingRelic(mirrorCharacter);
        if (starter == null || Owner.Relics.Any(r => r.Id == starter.Id)) return;

        await RelicCmd.Obtain(starter.ToMutable(), Owner);
    }

    // Modelled on PrismaticGem, which widens the reward pool the same way.
    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (Owner != player) return options;
        if (options.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications)) return options;
        if (!options.Flags.HasFlag(CardCreationFlags.IsCardReward)) return options;
        if (options.CardPools.All(p => p.IsColorless)) return options;

        bool onBeta = int.Parse((ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? "v0.0.0").Split('.')[1]) > 107;

        var mirrorPools = AltCharacterUtil.GetMirrorCharacters(player.Character).Select(c => c.CardPool);

        // The mirror pool contains duplicates of cards we already have, which would otherwise show up twice and roll
        // at double odds. WithFilter overwrites, so keep whatever filter was already there.
        var bannedIds = AltCharacterUtil.GetBannedCardIds(player.Character);

        if (onBeta) {
            var WithPoolsMethod = options.GetType().GetMethod("WithCardPools", [typeof(IEnumerable<CardPoolModel>)]);
            var WithFilterMethod = options.GetType().GetMethod("WithFilter", [typeof(Func<CardModel, bool>)]);

            if (WithPoolsMethod != null) {
                options = (CardCreationOptions)WithPoolsMethod.Invoke(options, [options.CardPools.Union(mirrorPools)])!;
            }

            if (bannedIds.Count > 0 && WithFilterMethod != null) {
                var existingFilter = options.CardPoolFilter;
                Func<CardModel, bool> newFilter = c => !bannedIds.Contains(c.Id) && (existingFilter == null || existingFilter(c));
                options = (CardCreationOptions)WithFilterMethod.Invoke(options, [newFilter])!;
            }
        } else {
            var cardPool = options.CardPools.Union(mirrorPools).SelectMany(pool =>
                    pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
                    .Where(c => !bannedIds.Contains(c.Id))
                    .ToArray();

            if (cardPool.Length == 0) {
                cardPool = player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                    .Where(c => !bannedIds.Contains(c.Id)).ToArray();
            }

            var WithCustomPoolMethod = options.GetType().GetMethod("WithCustomPool", [typeof(IEnumerable<CardModel>), typeof(CardRarityOddsType?)]);
            if (WithCustomPoolMethod != null)
            {
                options = (CardCreationOptions)WithCustomPoolMethod.Invoke(options, [cardPool, null])!;
            }
        }

        return options;
    }
}
