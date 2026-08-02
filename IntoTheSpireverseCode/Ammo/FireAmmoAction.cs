using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public class FireAmmoAction : GameAction
{
    private readonly Player _player;

    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public FireAmmoAction(Player player)
    {
        _player = player;
    }

    protected override async Task ExecuteAction()
    {
        if (!await FireAmmoCmd.Fire(_player, chargeEnergy: true))
            Cancel();
    }

    public override INetAction ToNetAction() => new NetFireAmmoAction();
    public override string ToString() => $"FireAmmoAction for player {_player.NetId}";
}
