using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Potions.Necrobinder;

[Pool(typeof(ShadowNecrobinderPotionPool))]
public class SNecroDeathglarePotion : IntoTheSpireversePotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Deathglare>(),
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var deathglare = Owner.Creature.CombatState.CreateCard<Deathglare>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
            deathglare, PileType.Draw, Owner, CardPilePosition.Random));
    }
}