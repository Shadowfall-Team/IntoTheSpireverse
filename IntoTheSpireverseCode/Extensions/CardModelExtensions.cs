using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

public static class CardModelExtensions
{
    public static void ApplyEnchantmentStatsToPreview(this CardModel card)
    {
        if (card.Enchantment is not { } enchantment)
            return;

        foreach (var dynamicVar in card.DynamicVars.Values.ToArray())
        {
            if (dynamicVar is BlockVar or CalculatedBlockVar)
            {
                var block = dynamicVar.BaseValue;
                block += enchantment.EnchantBlockAdditive(block);
                block *= enchantment.EnchantBlockMultiplicative(block);
                dynamicVar.BaseValue = block;

                continue;
            }

            var props = dynamicVar switch
            {
                DamageVar dmg => dmg.Props,
                OstyDamageVar osty => osty.Props,
                CalculatedDamageVar calc => calc.Props,
                _ => (ValueProp?)null,
            };

            if (props is { } valueProps)
            {
                var damage = dynamicVar.BaseValue;
                damage += enchantment.EnchantDamageAdditive(damage, valueProps);
                damage *= enchantment.EnchantDamageMultiplicative(damage, valueProps);
                dynamicVar.BaseValue = damage;
            }
        }
    }
}
