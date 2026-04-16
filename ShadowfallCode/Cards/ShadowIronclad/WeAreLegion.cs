using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class WeAreLegion() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selection = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => c.Type == CardType.Attack, this)).FirstOrDefault();

        if (selection == null)
            return;

        var toTransform = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != null && c.IsTransformable && c.Type != CardType.Attack)
            .ToList();

        foreach (var original in toTransform)
        {
            var clone = selection.CreateClone();
            await CardCmd.Transform(original, clone);
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}