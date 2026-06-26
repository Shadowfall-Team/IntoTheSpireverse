using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

public sealed class Gravellize() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SpikedRock>(),
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1)
        {
            ShouldGlowGold = c => c.Keywords.Contains(CardKeyword.Unplayable)
        };
        var original = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        if (original == null) return;

        var wasUnplayable = original.Keywords.Contains(CardKeyword.Unplayable);

        var spikedRock = CombatState!.CreateCard<SpikedRock>(Owner);
        await CardCmd.Transform(original, spikedRock);

        if (wasUnplayable)
        {
            var bonusRock = CombatState.CreateCard<SpikedRock>(Owner);
            await CardPileCmd.AddGeneratedCardsToCombat([bonusRock], PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}