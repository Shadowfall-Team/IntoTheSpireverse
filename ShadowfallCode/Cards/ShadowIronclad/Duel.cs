using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Duel() : ShadowIroncladCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
        new DamageVar(9m, ValueProp.Move),
    ];
    
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Count() <= 1;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        async Task GainBlock() => await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        async Task Hit() => await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await GainBlock();
        await Hit();

        bool isAlone = !CombatState.GetTeammatesOf(cardPlay.Target)
            .Where(e => e != cardPlay.Target && e.IsHittable)
            .Any();

        if (isAlone)
        {
            await GainBlock();
            await Hit();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}