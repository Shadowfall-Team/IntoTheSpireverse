using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Shadowfall.ShadowfallCode.Rewards;

namespace Shadowfall.ShadowfallCode.Patches;

public static class RewardSynchronizerPatches
{
    extension(RewardSynchronizer rewardSynchronizer)
    {
        /// <summary>
        /// Method to handle transforming a card as a combat reward
        /// </summary>
        public async Task<bool> DoLocalCardTransform(int amount = 1, bool upgrade = false)
        {
            CardTransformRewardMessage message = new CardTransformRewardMessage
            {
                Location = rewardSynchronizer._messageBuffer.CurrentLocation,
                Upgrade = upgrade,
                Amount = amount,
                SenderId = rewardSynchronizer.LocalPlayer.NetId
            };
            MainFile.Logger.Debug($"Transforming card for local player {rewardSynchronizer.LocalPlayer}");

            CustomTargetedMessageWrapper.Send(message);
            return await rewardSynchronizer.DoCardTransform(rewardSynchronizer.LocalPlayer, amount, upgrade);
        }

        /// <summary>
        /// Transform a card for a specific player as a combat reward
        /// </summary>
        public async Task<bool> DoCardTransform(Player player, int amount = 1, bool upgrade = false)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(
                    upgrade
                        ? CardSelectorPrefsExtensions.TransformAndUpgradeSelectionPrompt
                        : CardSelectorPrefs.TransformSelectionPrompt,
                    1,
                    amount)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            };

            // Need to check this doesn't desync if the message gets sent to the source player?
            List<CardModel> cards = (await CardSelectCmd.FromDeckForTransformation(player, prefs)).ToList();

            MainFile.Logger.Debug($"Current combat state for transform rewards is: IsEnding={CombatManager.Instance.IsEnding}");
            foreach (CardModel card in cards)
            {
                CardModel newCard = CardFactory.CreateRandomCardForTransform(
                        card,
                        isInCombat: false,
                        player.RunState.Rng.Niche);

                if (upgrade || card.IsUpgraded) // need a more robust handler for multi-upgrade at some point
                {
                    CardCmd.Upgrade(newCard);
                }

                await CardCmd.Transform(card, newCard, CardPreviewStyle.GridLayout);
                MainFile.Logger.Debug($"Player {player.NetId} transformed {card.Id} in their deck into {newCard.Id}" + (upgrade ? " and upgraded it." : "."));
            }

            return cards.Count > 0;
        }

        /// <summary>
        /// Method to handle transforming a card as a combat reward
        /// </summary>
        public async Task<bool> DoLocalCardUpgrade(int amount = 1, bool upgrade = false)
        {
            CardUpgradeRewardMessage message = new CardUpgradeRewardMessage
            {
                Location = rewardSynchronizer._messageBuffer.CurrentLocation,
                Amount = amount,
                SenderId = rewardSynchronizer.LocalPlayer.NetId
            };

            CustomTargetedMessageWrapper.Send(message);
            return await rewardSynchronizer.DoCardUpgrade(rewardSynchronizer.LocalPlayer, amount);
        }

        public async Task<bool> DoCardUpgrade(Player player, int amount = 1)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(
                    CardSelectorPrefs.UpgradeSelectionPrompt,
                    1,
                    amount)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            };

            List<CardModel> cards = (await CardSelectCmd.FromDeckForUpgrade(player, prefs)).ToList();

            CardCmd.Upgrade(cards, CardPreviewStyle.GridLayout); // grid because horizontal is behind the reward overlay

            return cards.Count > 0;
        }
    }
}
