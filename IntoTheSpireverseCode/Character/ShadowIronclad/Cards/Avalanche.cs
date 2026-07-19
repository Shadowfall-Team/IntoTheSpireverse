using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Avalanche() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_rock_shatter", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, 67676767);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => c.IsTransformable, this)).ToList();
        var rockPool = ModelDb.AllCards
            .Where(c => c.Tags.Contains(IntoTheSpireverseCardTags.Rock))
            .ToList();
        foreach (var original in selected)
        {
            var template = Owner.RunState.Rng.CombatCardGeneration.NextItem(rockPool);
            if (template == null) continue;
            var rock = CombatState.CreateCard(template, Owner);
            if (IsUpgraded)
                CardCmd.Upgrade(rock);
            await CardCmd.Transform(original, rock);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}