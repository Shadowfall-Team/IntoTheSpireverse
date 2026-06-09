using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace Shadowfall.ShadowfallCode.Ammo;

public struct NetPlayAmmoCardAction : INetAction, IPacketSerializable
{
    public GameAction ToGameAction(Player player) => new PlayAmmoCardAction(player);

    public void Serialize(PacketWriter writer)
    {
        // resolved from sender context
    }

    public void Deserialize(PacketReader reader)
    {
    }

    public override string ToString() => nameof(NetPlayAmmoCardAction);
}
