using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class FairyDust() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string SwiftAmountKey = "SwiftAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(SwiftAmountKey, 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip>();
            tips.Add(HoverTipFactory.FromCard<Ward>(IsUpgraded));
            tips.AddRange(HoverTipFactory.FromEnchantment<Swift>(DynamicVars[SwiftAmountKey].IntValue));
            return tips;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var wards = new CardModel[]
        {
            CombatState.CreateCard<Ward>(Owner),
            CombatState.CreateCard<Ward>(Owner),
        };

        await CardPileCmd.AddGeneratedCardsToCombat(wards, PileType.Hand, Owner);

        foreach (var ward in wards)
        {
            if (IsUpgraded)
                CardCmd.Upgrade(ward);
            CardCmd.Enchant<Swift>(ward, DynamicVars[SwiftAmountKey].BaseValue);
        }
    }

    protected override void OnUpgrade() { }
}