using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Relics.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class Warp() : CustomCardModel(0,
    CardType.Skill,
    CardRarity.Token,
    TargetType.Self)
{
    public override string CustomPortraitPath => $"res://Shadowfall/images/card_portraits/regent/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        
            // [
                // new PowerVar<StrengthPower>(
                    // 0),
                // new EnergyVar(3)
            // ]
            // :
            [
                new PowerVar<StrengthPower>(1),
                new EnergyVar(2)
            ];

    public override void AfterCreated()
    {
        if (!Owner.Relics.OfType<HulaFigure>().Any()) return;
        DynamicVars.Strength.BaseValue -= 1;
        DynamicVars.Energy.BaseValue += 1;
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.Strength.BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1);
    }
}
