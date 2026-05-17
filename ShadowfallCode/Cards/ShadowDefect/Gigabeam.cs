using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Gigabeam() : ShadowDefectCard(2, CardType.Attack, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(26M, ValueProp.Move),
    };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Void>(),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async () =>
            {
                await Task.CompletedTask;
            })
            .Execute(choiceContext);

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
            CombatState.CreateCard<Void>(Owner),
            PileType.Draw,
            Owner,
            CardPilePosition.Top));

        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6M);
    }
}

