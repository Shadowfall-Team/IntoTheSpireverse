using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.Cards;

public sealed class RipAndTear() : ShadowDefectCard(2, CardType.Attack, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6M, ValueProp.Move),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(3)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_slash")
            .TargetingRandomOpponents(CombatState)
            .Execute(choiceContext);

        CardModel card = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            (AbstractModel) this)).FirstOrDefault<CardModel>()!;

        if (card == null)
            return;

        await CardCmd.Discard(choiceContext, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}