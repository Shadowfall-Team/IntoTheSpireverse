using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class ShadowLunarPastry : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ShardPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShardPower>()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext,
        Player player)
    {
        await PowerCmd.Apply<ShardPower>(Owner.Creature,
            DynamicVars[nameof(ShardPower)].BaseValue,
            Owner.Creature,
            null);
    }
}