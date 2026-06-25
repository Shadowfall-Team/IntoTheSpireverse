using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class ExaltedFormPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext,
        Player player)
    {
        if (Owner.Player == null || player != Owner.Player) return;

        for (var i = 0; i < Amount; i++)
        {
            var discovery = CombatState.CreateCard<Discovery>(Owner.Player);
            discovery.SetToFreeThisCombat();
            await CardPileCmd.AddGeneratedCardToCombat(discovery, PileType.Hand, Owner.Player);
        }
    }
}