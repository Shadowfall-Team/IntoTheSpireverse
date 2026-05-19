using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Gigabeam() : ShadowDefectCard(2, CardType.Attack, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(26, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
   
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState!)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async delegate
            {
                List<Creature> enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();
                NHyperbeamVfx? nhyperbeamVfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
                if (nhyperbeamVfx != null)
                {
                    NCombatRoom? instance = NCombatRoom.Instance;
                    instance?.CombatVfxContainer.AddChildSafely(nhyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }
                foreach (Creature creature in enemies)
                {
                    NHyperbeamImpactVfx? nhyperbeamImpactVfx = NHyperbeamImpactVfx.Create(Owner.Creature, creature);
                    if (nhyperbeamImpactVfx == null) continue;
                    NCombatRoom? instance2 = NCombatRoom.Instance;
                    instance2?.CombatVfxContainer.AddChildSafely(nhyperbeamImpactVfx);
                }
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
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}

