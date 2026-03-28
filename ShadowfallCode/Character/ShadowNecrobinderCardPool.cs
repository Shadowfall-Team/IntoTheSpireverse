using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Reanimate = Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder.Reanimate;

namespace Shadowfall.ShadowfallCode.Character;



public class ShadowNecrobinderCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_necrobinder";
    public override string EnergyColorName => "necrobinder";

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load Shadowfall/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
			ModelDb.Card<Afterlife>(),
			ModelDb.Card<BansheesCry>(),
			ModelDb.Card<BlightStrike>(),
			ModelDb.Card<Bodyguard>(),
			ModelDb.Card<BoneShards>(),
			ModelDb.Card<BorrowedTime>(),
			ModelDb.Card<Bury>(),
			ModelDb.Card<Calcify>(),
			ModelDb.Card<CallOfTheVoid>(),
			ModelDb.Card<CaptureSpirit>(),
			ModelDb.Card<Cleanse>(),
			ModelDb.Card<Countdown>(),
			ModelDb.Card<DanseMacabre>(),
			ModelDb.Card<DeathMarch>(),
			ModelDb.Card<Deathbringer>(),
			ModelDb.Card<DeathsDoor>(),
			ModelDb.Card<Debilitate>(),
			ModelDb.Card<DefendNecrobinder>(),
			ModelDb.Card<Defile>(),
			ModelDb.Card<Defy>(),
			ModelDb.Card<Delay>(),
			ModelDb.Card<Demesne>(),
			ModelDb.Card<DevourLife>(),
			ModelDb.Card<Dirge>(),
			ModelDb.Card<DrainPower>(),
			ModelDb.Card<Dredge>(),
			ModelDb.Card<Eidolon>(),
			ModelDb.Card<EndOfDays>(),
			ModelDb.Card<EnfeeblingTouch>(),
			ModelDb.Card<Eradicate>(),
			ModelDb.Card<Fear>(),
			ModelDb.Card<Fetch>(),
			ModelDb.Card<Flatten>(),
			ModelDb.Card<ForbiddenGrimoire>(),
			ModelDb.Card<Friendship>(),
			ModelDb.Card<GlimpseBeyond>(),
			ModelDb.Card<GraveWarden>(),
			ModelDb.Card<Graveblast>(),
			ModelDb.Card<Hang>(),
			ModelDb.Card<Haunt>(),
			ModelDb.Card<HighFive>(),
			ModelDb.Card<Invoke>(),
			ModelDb.Card<LegionOfBone>(),
			ModelDb.Card<Lethality>(),
			ModelDb.Card<Melancholy>(),
			ModelDb.Card<Misery>(),
			ModelDb.Card<NecroMastery>(),
			ModelDb.Card<NegativePulse>(),
			ModelDb.Card<Neurosurge>(),
			ModelDb.Card<NoEscape>(),
			ModelDb.Card<Oblivion>(),
			ModelDb.Card<Pagestorm>(),
			ModelDb.Card<Parse>(),
			ModelDb.Card<Poke>(),
			ModelDb.Card<Protector>(),
			ModelDb.Card<PullAggro>(),
			ModelDb.Card<PullFromBelow>(),
			ModelDb.Card<Putrefy>(),
			ModelDb.Card<Rattle>(),
			ModelDb.Card<Reanimate>(),
			ModelDb.Card<Reap>(),
			ModelDb.Card<ReaperForm>(),
			ModelDb.Card<Reave>(),
			ModelDb.Card<RightHandHand>(),
			ModelDb.Card<Sacrifice>(),
			ModelDb.Card<Scourge>(),
			ModelDb.Card<SculptingStrike>(),
			ModelDb.Card<Seance>(),
			ModelDb.Card<SentryMode>(),
			ModelDb.Card<Severance>(),
			ModelDb.Card<SharedFate>(),
			ModelDb.Card<Shroud>(),
			ModelDb.Card<SicEm>(),
			ModelDb.Card<SleightOfFlesh>(),
			ModelDb.Card<Snap>(),
			ModelDb.Card<SoulStorm>(),
			ModelDb.Card<Sow>(),
			ModelDb.Card<SpiritOfAsh>(),
			ModelDb.Card<Spur>(),
			ModelDb.Card<Squeeze>(),
			ModelDb.Card<StrikeNecrobinder>(),
			ModelDb.Card<TheScythe>(),
			ModelDb.Card<TimesUp>(),
			ModelDb.Card<Transfigure>(),
			ModelDb.Card<Undeath>(),
			ModelDb.Card<Unleash>(),
			ModelDb.Card<Veilpiercer>(),
			ModelDb.Card<Wisp>()
        ];
    }
}
