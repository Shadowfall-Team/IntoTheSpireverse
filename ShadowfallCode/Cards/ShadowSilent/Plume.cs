using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Plume() : ShadowSilentCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BleedPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BleedPower>(cardPlay.Target, DynamicVars[nameof(BleedPower)].BaseValue, Owner.Creature, this);
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Skill)
            return;

        int skillsThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.HappenedThisTurn(CombatState)
                        && e.CardPlay.Card.Owner == Owner
                        && e.CardPlay.Card.Type == CardType.Skill);

        if (skillsThisTurn > 0 && skillsThisTurn % 3 == 0)
        {
            CardPile pile = Pile;
            if (pile != null && pile.Type == PileType.Discard)
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BleedPower)].UpgradeValueBy(1m);
    }
}
