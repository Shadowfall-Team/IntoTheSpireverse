using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Rewards;

public sealed class CardUpgradeRewardMessage : CustomRewardMessage
{
    internal void HandleCardUpgradeMessage(CardUpgradeRewardMessage message, ulong senderId)
    {
        MainFile.Logger.Debug($"Handling message {message}");
        var rs = RunManager.Instance.RewardSynchronizer;
        if (CombatManager.Instance.IsInProgress)
        {
            rs.BufferCustomRewardMessage(message, senderId);
            return;
        }

        Player? player = rs.PlayerCollection()?.GetPlayer(senderId);
        if (player == rs.LocalPlayerRef())
        {
            throw new InvalidOperationException("CardUpgradeRewardMessage shouldn't be sent to the player doing the upgrade!");
        }
        TaskHelper.RunSafely(rs.DoCardUpgrade(player, message.Amount));
    }

    public override void Dispose(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.UnregisterMessageHandler<CardUpgradeRewardMessage>(HandleCardUpgradeMessage);
    }

    public override void Initialize(RunLocationTargetedMessageBuffer messageBuffer)
    {
        messageBuffer.RegisterMessageHandler<CardUpgradeRewardMessage>(HandleCardUpgradeMessage);
    }

    public required int Amount;
    public override LogLevel LogLevel => LogLevel.Debug;

    public override void Serialize(PacketWriter writer)
    {
        writer.Write(Location);
        writer.WriteInt(Amount);
    }

    public override void Deserialize(PacketReader reader)
    {
        Location = reader.Read<RunLocation>();
        Amount = reader.ReadInt();
    }


}
