using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Eventide() : ShadowDefectCard(1, CardType.Attack, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8M, ValueProp.Move),
        new RepeatVar(2)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Void>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_slash")
            .TargetingRandomOpponents(CombatState)
            .Execute(choiceContext);

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
            CombatState.CreateCard<Void>(Owner),
            PileType.Draw,
            Owner, CardPilePosition.Top));

        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade() { }
}
