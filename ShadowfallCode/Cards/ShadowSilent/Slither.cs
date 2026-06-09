using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Slither() : ShadowSilentCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<Slippery2Power>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Slippery2Power>(),
        HoverTipFactory.FromCard<Weight>(false),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        await PowerCmd.Apply<Slippery2Power>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<Slippery2Power>().BaseValue,
            Owner.Creature, this);

        var weights = new CardModel[]
        {
            CombatState.CreateCard<Weight>(Owner),
            CombatState.CreateCard<Weight>(Owner),
        };
        await CardPileCmd.AddGeneratedCardsToCombat(weights, PileType.Hand, Owner);
    }

    protected override void OnUpgrade() =>
        AddKeyword(CardKeyword.Retain);
}