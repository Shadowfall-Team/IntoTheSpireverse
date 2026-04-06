using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class AssemblyRequired() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (CombatState == null) return;

        var players = CombatState.Players.Where(p => p.Creature.IsAlive);
        foreach (var player in players)
        {
            var tripCard = CombatState.CreateCard<Fragment>(player);
            await CardPileCmd.Add(tripCard, PileType.Draw, CardPilePosition.Random, this);
        }
    }

    protected override void OnUpgrade()
    {
        //TODO: what should the upgrade be?
    }
}