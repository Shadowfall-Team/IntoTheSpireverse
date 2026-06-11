using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class AncestralEcho() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private static readonly string _replayKey = "Replay";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar(_replayKey, 1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.ReplayStatic)] ;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        IReadOnlyList<CardModel> cards = PileType.Draw.GetPile(Owner).Cards;
		CardModel? cardModel = null;
		if (Owner.PlayerCombatState?.TurnNumber == 1)
		{
			cardModel = cards.FirstOrDefault((CardModel c) => !c.Keywords.Contains(CardKeyword.Innate));
		}
		if (cardModel == null)
		{
			cardModel = cards.FirstOrDefault();
		}
		if (cardModel != null)
		{
			cardModel.BaseReplayCount += DynamicVars[_replayKey].IntValue;
            CardCmd.Preview(cardModel);
		}
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
