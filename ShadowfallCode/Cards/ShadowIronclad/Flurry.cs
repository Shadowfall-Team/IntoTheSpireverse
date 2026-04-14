using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Flurry() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string ShivCountKey = "ShivCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1m),
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar(ShivCountKey, 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        for (int i = 0; i < DynamicVars[ShivCountKey].IntValue; i++)
        {
            await Shiv.CreateInHand(Owner, CombatState);
            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade() => DynamicVars[ShivCountKey].UpgradeValueBy(1m);
}