using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public class AmmoState
{
    public int Ammo { get; set; }
    public AmmoVolley PhantomCard { get; init; } = null!;
}
