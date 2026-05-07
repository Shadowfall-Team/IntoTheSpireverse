using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Rewards;

public sealed class CardUpgradeRewardMessage : ICustomTargetedMessage
{
    public void HandleMessage()
    {
        var rs = RunManager.Instance.RewardSynchronizer;

        Player? player = rs._playerCollection?.GetPlayer(SenderId);
        if (player == rs.LocalPlayer)
        {
            throw new InvalidOperationException("CardUpgradeRewardMessage shouldn't be sent to the player doing the upgrade!");
        }

        TaskHelper.RunSafely(rs.DoCardUpgrade(player, Amount));
    }

    // Pre-emptive method signiature for next update
    public void HandleMessage(ulong senderId)
    {
        var rs = RunManager.Instance.RewardSynchronizer;

        Player? player = rs._playerCollection?.GetPlayer(senderId);
        if (player == rs.LocalPlayer)
        {
            throw new InvalidOperationException("CardUpgradeRewardMessage shouldn't be sent to the player doing the upgrade!");
        }

        TaskHelper.RunSafely(rs.DoCardUpgrade(player, Amount));
    }

    public required int Amount;
    public required ulong SenderId; // remove this with next baselib update
    public LogLevel LogLevel => LogLevel.Debug;

    public bool IsRewardMessage => true;

    public RunLocation Location { get; set; }

    public bool ShouldBroadcast => true;

    public void Serialize(PacketWriter writer)
    {
        writer.Write(Location);
        writer.WriteInt(Amount);
        writer.WriteULong(SenderId);
    }

    public void Deserialize(PacketReader reader)
    {
        Location = reader.Read<RunLocation>();
        Amount = reader.ReadInt();
        SenderId = reader.ReadULong();
    }

}
