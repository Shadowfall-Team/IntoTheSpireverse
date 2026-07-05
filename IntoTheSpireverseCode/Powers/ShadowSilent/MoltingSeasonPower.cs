
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class MoltingSeasonPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player)
            return;
        Flash();
        
        var scales = Enumerable.Range(0, Amount)
            .Select(_ => CombatState.CreateCard<Scale>(Owner.Player))
            .ToArray();
        
        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, Owner.Player);
    }

}