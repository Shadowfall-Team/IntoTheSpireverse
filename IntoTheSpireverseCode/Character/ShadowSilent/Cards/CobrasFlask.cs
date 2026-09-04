using MegaCrit.Sts2.Core.Animation;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class CobrasFlask() : ShadowSilentCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private readonly Color _vfxTint = new Color("83eb85");
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(5m),
        new DynamicVar(_deviousKey, 0m),
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (NCombatRoom.Instance == null) return;
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        var node = NCombatRoom.Instance.GetCreatureNode(Owner.Creature);
        if (node == null) return;
        Vector2 lastPos = node.VfxSpawnPosition;
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, async() =>
        {
            var enemy = cardPlay.Card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (enemy != null)
            {
                 if (TestMode.IsOff)
                 {
                     var targetNode = NCombatRoom.Instance.GetCreatureNode(enemy);
                     if (targetNode != null)
                     {
                         NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NItemThrowVfx.Create(lastPos,
                             targetNode.GetBottomOfHitbox(), ModelDb.Potion<PoisonPotion>().Image));
                         lastPos = targetNode.VfxSpawnPosition;
                         await Cmd.Wait(0.5f);
                         NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                             NSplashVfx.Create(targetNode.VfxSpawnPosition, _vfxTint));
                         NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                             NLiquidOverlayVfx.Create(enemy, _vfxTint));
                         NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                             NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, _vfxTint));
                     }
                 } 
                 await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, DynamicVars.Poison.BaseValue, cardPlay.Card.Owner.Creature, this);
            }
        });
    }

    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}
