using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Shadowfall.Orbs;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Trinvoke() : ShadowDefectCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var source = this;
        
        await CreatureCmd.TriggerAnim(source.Owner.Creature, "Cast", source.Owner.Character.CastAnimDelay);
        
        foreach (OrbModel orb in (IEnumerable<OrbModel>) source.Owner.PlayerCombatState!.OrbQueue.Orbs)
        {
            for (int i = 0; i < 3; i++)
            {
                await OrbCmd.Passive(choiceContext, orb, null);
                await Cmd.Wait(0.1f);
            }
        }
    }
    
    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}