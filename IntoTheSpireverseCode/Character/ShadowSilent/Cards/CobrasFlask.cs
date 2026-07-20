using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class CobrasFlask() : ShadowSilentCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private readonly Color _vfxTint = new Color("83eb85");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Devious,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        Vector2 lastPos = NCombatRoom.Instance.GetCreatureNode(Owner.Creature).VfxSpawnPosition;
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, async() =>
        {
            Creature enemy = cardPlay.Card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (enemy != null)
            {
                 if (TestMode.IsOff)
                {
                    NCreature targetNode = NCombatRoom.Instance.GetCreatureNode(enemy);
                    if (targetNode != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely((Node)NItemThrowVfx.Create(lastPos,
                            targetNode.GetBottomOfHitbox(), ModelDb.Potion<PoisonPotion>().Image));
                        lastPos = targetNode.VfxSpawnPosition;
                        await Cmd.Wait(0.5f);
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                            (Node)NSplashVfx.Create(targetNode.VfxSpawnPosition, _vfxTint));
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                            (Node)NLiquidOverlayVfx.Create(enemy, _vfxTint));
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(
                            (Node)NGaseousImpactVfx.Create(targetNode.VfxSpawnPosition, _vfxTint));
                    }
                } 
                await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, DynamicVars.Poison.BaseValue, cardPlay.Card.Owner.Creature, this);
            }
        });
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(2m);
    }
}
