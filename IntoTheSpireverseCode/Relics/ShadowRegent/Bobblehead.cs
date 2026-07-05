using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

public class Bobblehead : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromPower<StrengthPower>()
    ];


    private bool _usedThisTurn;

    public bool UsedThisTurn
    {
        get => _usedThisTurn;
        set => _usedThisTurn = value;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType,
        AbstractModel? source)
    {
        if (card.Pile != null && card.Pile.Type == CargoCardPile.CargoPileType &&
            card.Owner == Owner && !UsedThisTurn)
        {
            UsedThisTurn = true;
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
                DynamicVars.Strength.BaseValue, Owner.Creature, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) { return; }

        UsedThisTurn = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}
