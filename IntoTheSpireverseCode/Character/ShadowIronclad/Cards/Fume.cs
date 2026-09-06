using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Fume() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner, 1, CardPilePosition.Top, forceExhaust: false);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
