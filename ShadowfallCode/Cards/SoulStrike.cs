using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class SoulStrike() : CustomCardModel(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CardsVar(1),
    ];
    
    protected override HashSet<CardTag> CanonicalTags
    {
        get => new HashSet<CardTag>() { CardTag.Strike };
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    public static IEnumerable<SoulStrike> Create(Player owner, int amount, ICombatState combatState)
    {
        List<SoulStrike> list = new List<SoulStrike>();
        for (int i = 0; i < amount; i++)
            list.Add(combatState.CreateCard<SoulStrike>(owner));
        return list;
    }
    
    public static async Task<IEnumerable<SoulStrike>> CreateInHand(Player owner, int amount, ICombatState combatState)
    {
        var soulStrikes = SoulStrike.Create(owner, amount, combatState).ToList();
        await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>)soulStrikes, PileType.Hand, owner);
        return soulStrikes;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}