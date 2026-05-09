using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class Deathglare() : CustomCardModel(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        ShadowfallKeywords.Linger,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                CreateClone(), PileType.Draw, Owner, CardPilePosition.Random));
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
