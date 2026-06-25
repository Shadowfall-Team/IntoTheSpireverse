using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Variables;

public class LoadAmmoVar(decimal amount) : DynamicVar(Key, amount)
{
    public const string Key = "LoadAmmo";
}
