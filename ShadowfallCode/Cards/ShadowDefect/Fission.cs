using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public class Fission() : ShadowDefectCard (0, CardType.Skill,CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
 
    public override OrbEvokeType OrbEvokeType => OrbEvokeType.All;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(0)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
 
        int orbCount = base.Owner.PlayerCombatState.OrbQueue.Orbs.Count;
        if (orbCount == 0)
            return;
 
        if (IsUpgraded)
        {
            // Evoke all orbs (each EvokeNext with dequeue:true evokes and removes the front orb)
            for (int i = 0; i < orbCount; i++)
            {
                await OrbCmd.EvokeNext(choiceContext, base.Owner, dequeue: true);
                await Cmd.CustomScaledWait(0.1f, 0.25f);
            }
        }
        else
        {
            for (int i = 0; i < orbCount; i++)
            {
                var orb = Owner.PlayerCombatState.OrbQueue.Orbs.First();
                Owner.PlayerCombatState.OrbQueue.Remove(orb);
                NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.OrbManager?.EvokeOrbAnim(orb);
                orb.RemoveInternal();
            }
        }
 
        for (int i = 0; i < orbCount; i++)
        {
            await PlayerCmd.GainEnergy(1, base.Owner);
            await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        }
    }
}