using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

//Heal 3(4) HP. Gain 1(2) Dexterity. Add Trip into your hand.
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

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

        await PowerCmd.Apply<DexterityPower>(Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);


        //TODO: what is trip?

        // var salvoCard = CombatState.CreateCard<Trip>(Owner);
        // await CardPileCmd.Add(fuelCard, PileType.Hand, source: this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1);
        DynamicVars.Dexterity.UpgradeValueBy(1);
    }
}