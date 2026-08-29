using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder;

public class ShadowNecrobinderCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_necrobinder";
    public override string EnergyColorName => "necrobinder"; // may need to be copied to fix relics?

    public override string CardFrameMaterialPath => "shadow_necrobinder";
    public override Color DeckEntryCardColor => new("6B4658");
    public override Color EnergyOutlineColor => new("702D6F");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<CallOfTheVoid>(),
            ModelDb.Card<DanseMacabre>(),
            ModelDb.Card<DeathMarch>(),
            ModelDb.Card<Debilitate>(),
            ModelDb.Card<Defile>(),
            ModelDb.Card<Defy>(),
            ModelDb.Card<Delay>(),
            ModelDb.Card<Demesne>(),
            ModelDb.Card<DrainPower>(),
            ModelDb.Card<Dredge>(),
            ModelDb.Card<EnfeeblingTouch>(),
            ModelDb.Card<Fear>(),
            ModelDb.Card<Friendship>(),
            ModelDb.Card<Graveblast>(),
            ModelDb.Card<Lethality>(),
            ModelDb.Card<PullFromBelow>(),
            ModelDb.Card<Putrefy>(),
            ModelDb.Card<SculptingStrike>(),
            ModelDb.Card<SpiritOfAsh>(),
            ModelDb.Card<Transfigure>(),
            ModelDb.Card<Undeath>(),
            ModelDb.Card<Wisp>()
        ];
    }
}
