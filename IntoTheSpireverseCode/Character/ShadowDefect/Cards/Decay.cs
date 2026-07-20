using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.Orbs;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.Cards;

public sealed class Decay() : ShadowDefectCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var source = this;
        
        await CreatureCmd.TriggerAnim(source.Owner.Creature, "Cast", source.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<EntropyOrb>(choiceContext, source.Owner);
        await CycleCmd.Cycle(choiceContext, source.Owner);
    }
    
    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}