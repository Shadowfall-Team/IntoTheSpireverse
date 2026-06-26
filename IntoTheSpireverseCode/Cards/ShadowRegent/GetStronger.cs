using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class GetStronger() : ShadowRegentCard(
    0,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        var powerAmount = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            powerAmount += 1;
        }

        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),Owner.Creature,
            powerAmount,
            Owner.Creature,
            this);

        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),Owner.Creature,
            powerAmount,
            Owner.Creature,
            this);
    }
}