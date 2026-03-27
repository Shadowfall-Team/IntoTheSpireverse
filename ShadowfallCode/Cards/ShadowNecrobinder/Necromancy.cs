using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Necromancy() : ShadowNecrobinderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SoulStrike>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var targetPile = IsUpgraded ? PileType.Draw : PileType.Discard;
        var soulStrikes = PileType.Exhaust.GetPile(Owner).Cards
            .Where(c => c is SoulStrike)
            .ToList();

        foreach (var card in soulStrikes)
        {
            await CardPileCmd.Add(card, targetPile);
        }
    }

    protected override void OnUpgrade() { }
}