using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowNecrobinder;

public class SNecroCurseHeal : ShadowNecrobinderRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(8m),
    ];

    public override async Task AfterCardChangedPiles(
        CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Pile?.Type != PileType.Deck) return;
        if (card.Owner != Owner) return;
        if (card.Type != CardType.Curse) return;
        if (Owner.Creature.IsDead) return;

        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}