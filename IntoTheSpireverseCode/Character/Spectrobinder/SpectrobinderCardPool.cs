using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.Spectrobinder;

public class SpectrobinderCardPool : CustomCardPoolModel
{
    public override string Title => "spectrobinder";
    public override string EnergyColorName => Spectrobinder.CharacterId;

    public override string CardFrameMaterialPath => "shadow_necrobinder";
    public override Color DeckEntryCardColor => new("6B4658");
    public override Color EnergyOutlineColor => new("702D6F");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<StrikeNecrobinder>(),
            ModelDb.Card<DefendNecrobinder>(),

            ModelDb.Card<DeathMarch>(),
            ModelDb.Card<Delay>(),
            ModelDb.Card<Eradicate>(),
            ModelDb.Card<Friendship>(),
            ModelDb.Card<Graveblast>(),
            ModelDb.Card<GraveWarden>(),
            ModelDb.Card<Haunt>(),
            ModelDb.Card<Melancholy>(),
            ModelDb.Card<Neurosurge>(),
            ModelDb.Card<Putrefy>(),
            ModelDb.Card<Reave>(),
            ModelDb.Card<Severance>(),
            ModelDb.Card<SoulStorm>(),
            ModelDb.Card<Undeath>(),

            // Multiplayer cards
            ModelDb.Card<Cacophony>(),
            ModelDb.Card<GlimpseBeyond>(),
            ModelDb.Card<Soulbound>(),
        ];
    }
}
