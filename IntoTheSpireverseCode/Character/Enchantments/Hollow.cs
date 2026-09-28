using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.Enchantments;

public sealed class Hollow : IntoTheSpireverseEnchantment
{
    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Ethereal);
        Card.RemoveKeyword(CardKeyword.Retain);
        if (Card.Type != CardType.Power)
            Card.AddKeyword(CardKeyword.Exhaust);
    }
}
