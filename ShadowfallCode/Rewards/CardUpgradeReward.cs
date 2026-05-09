using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;

namespace Shadowfall.ShadowfallCode.Rewards;

//TODO: Not sure if extra is needed for multiplayer. Plz playtest
public class CardUpgradeReward(Player player) : Reward(player)
{
    //TODO: add icon
    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_removal.png");

    protected override RewardType RewardType => RewardType.RemoveCard;

    public override int RewardsSetIndex => 8;

    protected override string IconPath => RewardIcon;

    public static IEnumerable<string> AssetPaths => new List<string>([RewardIcon]);

    public override bool IsPopulated => true;

    public override LocString Description => new("gameplay_ui", "COMBAT_REWARD_CARD_UPGRADE");

    public override void Populate() { }

    protected override async Task<bool> OnSelect()
    {
        var cardSelectorPrefs = new CardSelectorPrefs(new LocString("gameplay_ui", "COMBAT_REWARD_CARD_UPGRADE.selectionScreenPrompt"), 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var card = (await CardSelectCmd.FromDeckForUpgrade(player, cardSelectorPrefs)).FirstOrDefault();
        if (card != null)
        {
            CardCmd.Upgrade(card);
        }

        return true;
    }

    public override void MarkContentAsSeen()
    {
    }
}
