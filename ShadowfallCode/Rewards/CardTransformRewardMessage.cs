using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Rewards;

/// <summary>
/// Message for transforming a card from a new reward type
/// </summary>
public sealed class CardTransformRewardMessage : ICustomTargetedMessage
{
    public void HandleMessage()
    {
        var rs = RunManager.Instance.RewardSynchronizer;

        Player? player = rs._playerCollection.GetPlayer(SenderId);
        if (player == rs.LocalPlayer)
        {
            throw new InvalidOperationException("CardTransformRewardMessage should not be sent to the player transforming the card");
        }
        TaskHelper.RunSafely(rs.DoCardTransform(player, Amount, Upgrade));
    }

    // Pre-emptive method signiature for next update
    public void HandleMessage(ulong senderId)
    {
        var rs = RunManager.Instance.RewardSynchronizer;

        Player? player = rs._playerCollection.GetPlayer(senderId);
        if (player == rs.LocalPlayer)
        {
            throw new InvalidOperationException("CardTransformRewardMessage should not be sent to the player transforming the card");
        }
        TaskHelper.RunSafely(rs.DoCardTransform(player, Amount, Upgrade));
    }


    /// <summary>
    /// Whether to upgrade the card as well as transforming
    /// </summary>
    public required bool Upgrade;
    /// <summary>
    /// The amount of cards to select from
    /// </summary>
    public required int Amount;
    public required ulong SenderId; // remove this with next baselib update

    public bool IsRewardMessage => true;

    public RunLocation Location { get; set; }

    public bool ShouldBroadcast => true;

    public void Deserialize(PacketReader reader)
    {
        Location = reader.Read<RunLocation>();
        Amount = reader.ReadInt();
        Upgrade = reader.ReadBool();
        SenderId = reader.ReadULong();
    }

    public void Serialize(PacketWriter writer)
    {
        writer.Write(Location);
        writer.WriteInt(Amount);
        writer.WriteBool(Upgrade);
        writer.WriteULong(SenderId);
    }
}
