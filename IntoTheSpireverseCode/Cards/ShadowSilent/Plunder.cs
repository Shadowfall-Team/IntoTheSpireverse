using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Plunder() : ShadowSilentCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string CalculatedDrawKey = "CalculatedDraw";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new EnergyVar(2),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedDrawKey).WithMultiplier((card, _) =>
            (decimal)PileType.Hand.GetPile(card.Owner).Cards
                .Count(c => c != card && GetEffectiveCost(c, card.Owner) >= 2)),
    ];

    private static int GetEffectiveCost(CardModel card, Player owner)
    {
        if (card.EnergyCost.CostsX)
            return owner.PlayerCombatState?.Energy ?? 0;
        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        int energyBeforePlay = (Owner.PlayerCombatState?.Energy ?? 0)
                               + cardPlay.Resources.EnergySpent;

        int drawCount = PileType.Hand.GetPile(Owner).Cards
            .Count(c =>
            {
                if (c.EnergyCost.CostsX)
                    return energyBeforePlay >= DynamicVars.Energy.IntValue;
                return c.EnergyCost.GetWithModifiers(CostModifiers.All) >= DynamicVars.Energy.IntValue;
            });

        if (drawCount > 0)
            await CardPileCmd.Draw(choiceContext, (decimal)drawCount, Owner);
    }

    protected override void OnUpgrade() =>
        DynamicVars.Damage.UpgradeValueBy(3m);
}
