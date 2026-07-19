using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Banana() : ShadowRegentCard(1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    public override string CustomPortraitPath => $"res://IntoTheSpireverse/images/card_portraits/regent/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3),
        new PowerVar<DexterityPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Trip>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);

        var tripCard = CombatState.CreateCard<Trip>(Owner);
        var cardAdd = await CardPileCmd.AddGeneratedCardToCombat(tripCard, CargoCardPile.CargoPileType, Owner);
        CardCmd.PreviewCardPileAdd(cardAdd);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1);
        DynamicVars.Dexterity.UpgradeValueBy(1);
    }
}