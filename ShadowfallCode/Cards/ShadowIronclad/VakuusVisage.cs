using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class VakuusVisage() : ShadowIroncladCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const string ApparitionCountKey = "ApparitionCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
        new DynamicVar(ApparitionCountKey, 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Apparition>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Damage(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this);

        var cards = new List<CardModel>();
        for (int i = 0; i < DynamicVars[ApparitionCountKey].IntValue; i++)
        {
            cards.Add(CombatState.CreateCard<Apparition>(Owner));
        }
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, true, CardPilePosition.Random));
    }

    protected override void OnUpgrade() => DynamicVars[ApparitionCountKey].UpgradeValueBy(1m);
}