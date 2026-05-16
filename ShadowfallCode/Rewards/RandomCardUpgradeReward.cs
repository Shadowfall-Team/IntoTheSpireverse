using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;

namespace Shadowfall.ShadowfallCode.Rewards;

//TODO: Not sure if extra is needed for multiplayer. Plz playtest
public class RandomCardUpgradeReward(Player player)  : Reward(player)
{
    //TODO: add icon
    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_removal.png");

    protected override RewardType RewardType => RewardType.RemoveCard;

    public override int RewardsSetIndex => 8;

    protected override string IconPath => RewardIcon;

    public static IEnumerable<string> AssetPaths => new List<string>([RewardIcon]);

    public override bool IsPopulated => true;

    public override LocString Description => new("gameplay_ui", "COMBAT_REWARD_RANDOM_CARD_UPGRADE");

    public override void Populate() { }

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

    public override void MarkContentAsSeen()
    {
    }
}
