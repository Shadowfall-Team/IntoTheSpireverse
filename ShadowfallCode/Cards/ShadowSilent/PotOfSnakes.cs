using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class PotOfSnakes() : ShadowSilentCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string MuddleCountKey = "MuddleCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(MuddleCountKey, 1m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Muddle),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 2m, Owner);

        await ShadowfallKeywords.ApplyMuddleFromHandSelection(
            choiceContext, Owner, this,
            DynamicVars[MuddleCountKey].IntValue);
    }

    protected override void OnUpgrade() =>
        DynamicVars[MuddleCountKey].UpgradeValueBy(1m);
}