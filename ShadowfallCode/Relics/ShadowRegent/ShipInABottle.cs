using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class ShipInABottle : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ShardsPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShardsPower>(),
        HoverTipFactory.FromCard<Warp>()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner) return;
        await PowerCmd.Apply<ShardsPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars[nameof(ShardsPower)].BaseValue,
            Owner.Creature,
            null);
    }
}