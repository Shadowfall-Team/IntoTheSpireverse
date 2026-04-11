using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Banana() : ShadowRegentCard(1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3),
        new PowerVar<DexterityPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Trip>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

        await PowerCmd.Apply<DexterityPower>(Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);

        var tripCard = CombatState.CreateCard<Trip>(Owner);
        await CardPileCmd.Add(tripCard, PileType.Hand, source: this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1);
        DynamicVars.Dexterity.UpgradeValueBy(1);
    }
}