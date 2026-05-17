using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Shadowfall.ShadowfallCode.Cards;

public class SingleMind() : ShadowDefectCard(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FocusPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var orbs = Owner.PlayerCombatState.OrbQueue.Orbs;
        if (orbs.Count == 0) return;
        var first = orbs.First();
        
        if (Owner.PlayerCombatState.OrbQueue.Orbs.All(x => x.GetType() == first.GetType()))
        {
            await PowerCmd.Apply<FocusPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}