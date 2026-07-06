using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Scalemail() : ShadowSilentCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Scale>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int count = ResolveEnergyXValue();
        
        if (IsUpgraded)
        {
           count += 1;
        }

        var scales = new CardModel[count];

        for (var i = 0; i < count; i++)
        {
            scales[i] = CombatState.CreateCard<Scale>(Owner);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, Owner);
    }
}