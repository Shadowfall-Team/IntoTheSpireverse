using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class WeAreLegion() : ShadowIroncladCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
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

        var copies = new List<CardModel>(toTransform.Count);
        foreach (var original in toTransform)
        {
            var clone = selection.CreateClone();
            await CardCmd.Transform(original, clone);
            copies.Add(clone);
        }

        foreach (var copy in copies)
            await CardCmd.AutoPlay(choiceContext, copy, null);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}