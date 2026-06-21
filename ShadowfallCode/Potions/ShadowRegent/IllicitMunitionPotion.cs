using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Commands;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;
using Shadowfall.ShadowfallCode.utils;

namespace Shadowfall.ShadowfallCode.Potions.ShadowRegent;

public class IllicitMunitionPotion : ShadowRegentPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("LoadAmmo", 1),
        new IntVar("Shots", 1),
        new PowerVar<SiegePower>(2),
        new BlockVar(6, ValueProp.Unpowered),
        new PowerVar<FirepowerPower>(6)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        .. LoadAmmoHoverTip.FromLoadAmmo(),
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await LoadAmmoCmd.LoadAmmo(DynamicVars["LoadAmmo"].BaseValue, target.Player, null);

        await PowerCmd.Apply<SiegePower>(choiceContext, target, DynamicVars.Power<SiegePower>().BaseValue,
            Owner.Creature, null);

        var power = await PowerCmd.Apply<DefensiveCannonadePower>(
            choiceContext, target.Player.Creature,
            DynamicVars.Block.BaseValue,
            Owner.Creature,
            null);
        power?.ShotsRemaining = DynamicVars["Shots"].IntValue;
    }
}