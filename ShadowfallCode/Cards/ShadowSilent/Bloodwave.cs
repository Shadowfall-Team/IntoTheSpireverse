using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Bloodwave() : ShadowSilentCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BleedPower>(5m),
        new PowerVar<VulnerablePower>(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromCard<Slimed>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature creature in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BleedPower>(creature, DynamicVars[nameof(BleedPower)].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(creature, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Slimed>(Owner), PileType.Hand, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BleedPower)].UpgradeValueBy(2m);
    }
}
