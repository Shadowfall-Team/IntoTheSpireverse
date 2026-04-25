using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Resurrection() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature creature in CombatState.PlayerCreatures.Where(c => c != null && c.IsAlive).ToList())
        {
            var graveblast = CombatState.CreateCard<Graveblast>(creature.Player);
            graveblast.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(graveblast, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}