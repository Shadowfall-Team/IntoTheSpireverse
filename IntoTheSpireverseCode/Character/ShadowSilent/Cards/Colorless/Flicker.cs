using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public sealed class Flicker() : ShadowColorlessCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await IntoTheSpireverseKeywords.ApplyMuddleFromHandSelection(
            choiceContext, Owner, this);
    }

    protected override void OnUpgrade() =>
        RemoveKeyword(CardKeyword.Exhaust);
}