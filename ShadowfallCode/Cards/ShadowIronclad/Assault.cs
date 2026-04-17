using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Assault() : ShadowIroncladCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var drawPile = PileType.Draw.GetPile(Owner);
        var pulled = new List<CardModel>();

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
            var card = drawPile.Cards.FirstOrDefault();
            if (card == null) break;
            pulled.Add(card);
            await CardPileCmd.Add(card, PileType.Play);
        }

        var attacks = pulled.Where(c => c.Type == CardType.Attack).ToList();
        var rest = pulled.Where(c => c.Type != CardType.Attack).ToList();

        foreach (var card in attacks)
        {
            if (Owner.Creature.IsDead) break;
            await CardCmd.AutoPlay(choiceContext, card, null);
        }

        foreach (var card in rest)
        {
            await CardPileCmd.Add(card, PileType.Discard);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}