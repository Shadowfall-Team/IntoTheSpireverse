using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Shadowfall.ShadowfallCode.Rewards;

//TODO: Not sure if extra is needed for multiplayer. Plz playtest
public sealed class RandomCardUpgradeReward(Player player) : CustomReward(player)
{
    //TODO: add icon
    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_removal.png");
    public static IEnumerable<string> AssetPaths => new List<string>([RewardIcon]);
    protected override string IconPath => RewardIcon;
    public override LocString Description => new("gameplay_ui", "COMBAT_REWARD_RANDOM_CARD_UPGRADE");

    [CustomEnum] public static RewardType RandomCardUpgrade;
    protected override RewardType RewardType => RandomCardUpgrade;

    public override int RewardsSetIndex => 8;
    public override bool IsPopulated => true;

    public static RandomCardUpgradeReward CreateFromSerializable(SerializableReward save, Player player)
    {
        return new RandomCardUpgradeReward(player);
    }

    public override CreateRewardFromSave<CustomReward> DeserializeMethod => CreateFromSerializable;

    public override void Populate() { }

    // This seems fine as far as I can tell?
    protected override async Task<bool> OnSelect()
    {
        var cardsToUpgrade = PileType.Deck.GetPile(Player).Cards
            .Where(c => c.IsUpgradable).ToList()
            .StableShuffle(Player.RunState.Rng.Niche)
            .Take(1).FirstOrDefault();

        if (cardsToUpgrade != null)
        {
            CardCmd.Upgrade(cardsToUpgrade);
        }
        return true;
    }

    public override void MarkContentAsSeen() { }
}
