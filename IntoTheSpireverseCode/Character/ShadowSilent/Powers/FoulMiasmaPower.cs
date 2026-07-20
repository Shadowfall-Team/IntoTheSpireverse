
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class FoulMiasmaPower : ShadowPowerModel, IntoTheSpireverseKeywords.ICardMuddledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    public async Task AfterCardMuddled(ICombatState combatState, CardModel cardModel)
    {
        if (cardModel.Owner != Owner.Player) 
            return;
        Flash();
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
          NCreature creatureNode = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
          if (creatureNode != null)
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(creatureNode.VfxSpawnPosition, new Color("83eb85")));
        }
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, Amount, Owner, null);
            
    }
}