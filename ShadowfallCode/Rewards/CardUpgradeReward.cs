using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Rewards;

public sealed class CardUpgradeReward(Player player) : CustomReward(player)
{
    //TODO: add icon
    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_removal.png");
    protected override string IconPath => RewardIcon;
    public static IEnumerable<string> AssetPaths => new List<string>([RewardIcon]);

    public override LocString Description
    {
        get
        {
            LocString locString = new("gameplay_ui", "COMBAT_REWARD_CARD_UPGRADE");
            locString.Add("cards", Amount);
            return locString;
        }
    }

    [CustomEnum] public static RewardType CardUpgrade;
    protected override RewardType RewardType => CardUpgrade;

    public override int RewardsSetIndex => 8;
    public override bool IsPopulated => true;

    public required int Amount;

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward()
        {
            RewardType = CardUpgrade,
            GoldAmount = Amount // Hijacking the base values for now
        };
    }

    public static CardUpgradeReward CreateFromSerializable(SerializableReward save, Player player)
    {
        return new CardUpgradeReward(player)
        {
            Amount = save.GoldAmount
        };
    }

    public override CreateRewardFromSave<CustomReward> DeserializeMethod => CreateFromSerializable;

    public override void Populate() { }

    protected override async Task<bool> OnSelect()
    {
        MainFile.Logger.Debug("Obtained selected card upgrade from reward");
        return await RunManager.Instance.RewardSynchronizer.DoLocalCardUpgrade(Amount);
    }

    public override void MarkContentAsSeen() { }
}
