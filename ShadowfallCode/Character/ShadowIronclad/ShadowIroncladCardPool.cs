using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIroncladCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_ironclad";
    public override string EnergyColorName => "ironclad";

    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<Breakthrough>(),
            ModelDb.Card<Headbutt>(),
            ModelDb.Card<PommelStrike>(),
            ModelDb.Card<SetupStrike>(),
            ModelDb.Card<Thunderclap>(),
            ModelDb.Card<Bloodletting>(),
            ModelDb.Card<ShrugItOff>(),
            ModelDb.Card<BloodWall>(),
            ModelDb.Card<Spite>(),
            ModelDb.Card<Whirlwind>(),
            ModelDb.Card<Bludgeon>(),
            ModelDb.Card<Stomp>(),
            ModelDb.Card<Rage>(),
            ModelDb.Card<InfernalBlade>(),
            ModelDb.Card<Taunt>(),
            ModelDb.Card<FlameBarrier>(),
            ModelDb.Card<Inferno>(),
            ModelDb.Card<Inflame>(),
            ModelDb.Card<Rupture>(),
            ModelDb.Card<StoneArmor>(),
            ModelDb.Card<Conflagration>(),
            ModelDb.Card<TearAsunder>(),
            ModelDb.Card<Cascade>(),
            ModelDb.Card<Offering>(),
            ModelDb.Card<OneTwoPunch>(),
            ModelDb.Card<Impervious>(),
            ModelDb.Card<Tank>(),
            ModelDb.Card<Juggernaut>()
        ];
    }
}