using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Enchantments;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class MudIdol : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Polished>();
}