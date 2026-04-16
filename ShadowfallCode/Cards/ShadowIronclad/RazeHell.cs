using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class RazeHell() : ShadowIroncladCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card.Type == CardType.Attack && !card.EnergyCost.CostsX)
                card.EnergyCost.SetUntilPlayed(0);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}