using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class RendSpirit() : ShadowNecrobinderCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SoulStrike>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var soulStrikes = SoulStrike.Create(Owner, 3, CombatState).ToList();
        var drawResult = await CardPileCmd.AddGeneratedCardToCombat(soulStrikes[0], PileType.Draw, true, CardPilePosition.Random);
        var discardResult = await CardPileCmd.AddGeneratedCardToCombat(soulStrikes[1], PileType.Discard, true);
        await CardPileCmd.AddGeneratedCardToCombat(soulStrikes[2], PileType.Hand, true);
        CardCmd.PreviewCardPileAdd([drawResult, discardResult]);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}