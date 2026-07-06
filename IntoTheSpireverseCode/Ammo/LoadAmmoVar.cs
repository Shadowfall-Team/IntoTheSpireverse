using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public class LoadAmmoVar : DynamicVar
{
    public const string Key = "LoadAmmo";

    public LoadAmmoVar(string name, decimal loadAmmo) : base(name, loadAmmo)
    {
        // this.WithTooltip();
    }

    public LoadAmmoVar(decimal loadAmmo)
        : base(Key, loadAmmo)
    {
        //TODO: I would like to be able to do hovertips (as seen in @IntoTheSpireverseCode/Ammo/LoadAmmoHoverTip.cs),
        // but I can't figure out how to do it without knowing whether or not the player exists as a field within the model.
        // this.WithTooltip();
    }
}

public static class LoadAmmoVarExtension
{
    extension(DynamicVarSet vars)
    {
        public DynamicVar LoadAmmo => vars[LoadAmmoVar.Key];
    }
}