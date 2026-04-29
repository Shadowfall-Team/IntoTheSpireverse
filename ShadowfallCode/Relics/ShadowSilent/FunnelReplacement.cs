using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Relics;

// TODO NEEDS A NAME 
public class FunnelReplacement : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BleedPower>(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
    ];
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
      if (side != Owner.Creature.Side || combatState.RoundNumber > 1) 
      { 
          return; 
      }
      Flash();
      //TODO MAKE RED
      foreach (Creature hittableEnemy in Owner.Creature.CombatState.HittableEnemies) 
      { 
          NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSmokePuffVfx.Create(hittableEnemy, NSmokePuffVfx.SmokePuffColor.Green)); 
      }
      await Cmd.CustomScaledWait(0.2f, 0.4f);
      foreach (Creature hittableEnemy2 in Owner.Creature.CombatState.HittableEnemies) 
      { 
          await PowerCmd.Apply<BleedPower>(new ThrowingPlayerChoiceContext(), hittableEnemy2, base.DynamicVars["BleedPower"].IntValue, Owner.Creature, null);
      }
	}


}