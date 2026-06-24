using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using static IntoTheSpireverse.IntoTheSpireverseCode.CardPiles.AmmoCardPile;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

/// <summary>
/// Spends energy and plays the top AmmoVolley card from the ammo pile.
/// Replaces FireAmmoAction — damage logic now lives in AmmoVolley.OnPlay.
/// </summary>
public class PlayAmmoCardAction : GameAction
{
    private readonly Player _player;

    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public PlayAmmoCardAction(Player player)
    {
        _player = player;
    }

    protected override async Task ExecuteAction()
    {
        var topCard = AmmoPileType.GetPile(_player).Cards.ToList().FirstOrDefault();

        if (topCard == null)
        {
            Cancel();
            return;
        }

        var cost = topCard.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (_player.PlayerCombatState.Energy < cost)
        {
            Cancel();
            return;
        }

        await topCard.SpendResources();
        await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), topCard, null);
    }

    public override INetAction ToNetAction() => new NetPlayAmmoCardAction();

    public override string ToString() => $"PlayAmmoCardAction for player {_player.NetId}";
}
