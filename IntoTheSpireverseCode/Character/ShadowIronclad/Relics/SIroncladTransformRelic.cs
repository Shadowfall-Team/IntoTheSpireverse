using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Enchantments;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class MudIdol : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Polished>();
}