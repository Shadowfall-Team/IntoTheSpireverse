using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class FuelTheFire() : ShadowIroncladCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string FuelCountKey = "FuelCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(FuelCountKey, 3m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Fuel>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var cards = new List<CardModel>();
        for (int i = 0; i < DynamicVars[FuelCountKey].IntValue; i++)
        {
            cards.Add(CombatState.CreateCard<Fuel>(Owner));
        }
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, true, CardPilePosition.Random));
    }

    protected override void OnUpgrade() => DynamicVars[FuelCountKey].UpgradeValueBy(1m);
}