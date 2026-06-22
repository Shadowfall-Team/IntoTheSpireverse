using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowNecrobinder;

public class SNecroSoulStrikeEthereal : ShadowNecrobinderRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromCard<SoulStrike>(),
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!CanAffect(card)) return Task.CompletedTask;
        CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return Task.CompletedTask;
        foreach (var card in Owner.PlayerCombatState.AllCards)
        {
            if (CanAffect(card))
                CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
        }
        return Task.CompletedTask;
    }

    private bool CanAffect(CardModel card)
    {
        return card is SoulStrike && !card.Keywords.Contains(CardKeyword.Ethereal);
    }
}