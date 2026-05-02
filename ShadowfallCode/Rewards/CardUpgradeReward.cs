using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Rewards;

//TODO: Not sure if extra is needed for multiplayer. Plz playtest
public sealed class CardUpgradeReward(Player player) : CustomReward(player)
{
    //TODO: add icon
    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_removal.png");
    protected override string IconPath => RewardIcon;
    public static IEnumerable<string> AssetPaths => new List<string>([RewardIcon]);

    [CustomEnum] public static RewardType CardUpgrade;
    protected override RewardType RewardType => CardUpgrade;

    public override int RewardsSetIndex => 8;

    public override bool IsPopulated => true;
    public override LocString Description => new("gameplay_ui", "COMBAT_REWARD_CARD_UPGRADE");

    // This stores the type by default, gets handled by base game
    // public override SerializableReward ToSerializable()

    public CardUpgradeReward CreateFromSerializable(SerializableReward save, Player player)
    {
        return new CardUpgradeReward(player);
    }

    public override SerializableCustomReward<CustomReward> SerializeMethod => CreateFromSerializable;

    public override Task Populate()
    {
        return Task.CompletedTask;
    }

    protected override async Task<bool> OnSelect()
    {
        MainFile.Logger.Debug("Obtained selected card upgrade from reward");
        return await RunManager.Instance.RewardSynchronizer.DoLocalCardUpgrade(1);
    }

    public override void MarkContentAsSeen() { }
}
