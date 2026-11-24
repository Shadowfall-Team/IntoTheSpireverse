using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.Cards;

public sealed class RipAndTear() : ShadowDefectCard(2, CardType.Attack, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6M, ValueProp.Move),
        new SpireverseHitCountVar(3)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.SpireverseHitCount.IntValue)
            .FromCardCompatibility(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .TargetingRandomOpponents(CombatState)
            .Execute(choiceContext);

        CardModel card = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            this)).FirstOrDefault()!;

        if (card == null)
            return;

        await CardCmd.Discard(choiceContext, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}
