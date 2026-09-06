using MegaCrit.Sts2.Core.Audio.Debug;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;

[Pool(typeof(TokenCardPool))]
public sealed class GhostRock() : RockCardBase(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    private const string StrengthLossKey = "StrengthLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move),
        new DynamicVar(StrengthLossKey, 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.rockShatterPath, tmpSfx: TmpSfx.bluntAttack)
            .Execute(choiceContext);
        await PowerCmd.Apply<GhostRockTemporaryStrengthPower>(
            new ThrowingPlayerChoiceContext(),
            cardPlay.Target, DynamicVars[StrengthLossKey].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars[StrengthLossKey].UpgradeValueBy(2m);
}
