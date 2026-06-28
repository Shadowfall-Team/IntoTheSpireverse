using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TrialOfSkill() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        // why this not compile? HoverTipFactory.FromEnchantment<Steady>(),
        HoverTipFactory.FromCard<MinionSacrifice>(IsUpgraded)

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await PowerCmd.Apply<TrialOfWeaponryPowerPlus>(new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                1,
                Owner.Creature,
                this);
        }
        else
        {
            await PowerCmd.Apply<TrialOfWeaponryPower>(new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                1,
                Owner.Creature,
                this);
        }
        
    }

    protected override void OnUpgrade()
    {
       
    }
}

public class TrialOfWeaponryPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("SkillsPlayedThisTurn", 0)
    ];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars["SkillsPlayedThisTurn"].BaseValue = 0;
        StopPulsing();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress)
            {
                if (cardPlay.Card.Type == CardType.Skill)
                {
                    DynamicVars["SkillsPlayedThisTurn"].BaseValue++;
                    if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 2 == 0)
                    {
                        StartPulsing();
                    }

                    if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 3 == 0)
                    {
                        Flash();
                            
                        //TODO - Tried a few attempts here but couldn't get the right setup for multiple, modified, upgraded cards added via generated cards to combat.
                        var sacCards = Array.Empty<CardModel>();
                        for (int i = 0; i < Amount; i++)
                        {
                            var sacCard = CombatState.CreateCard<MinionSacrifice>(Owner.Player);
                            CardCmd.Enchant<Steady>(sacCard, 1);
                            sacCards.AddItem(sacCard);
                        }
                            
                        await CardPileCmd.AddGeneratedCardsToCombat(sacCards, PileType.Hand, Owner.Player);
                            
                        await PowerCmd.Remove(this);
                    }
                }
            }
        }
    }
}

public class TrialOfWeaponryPowerPlus : ShadowPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new IntVar("SkillsPlayedThisTurn", 0)
        ];

        public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != Owner.Side)
            {
                return Task.CompletedTask;
            }

            DynamicVars["SkillsPlayedThisTurn"].BaseValue = 0;
            StopPulsing();
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context,
            CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner.Creature == Owner)
            {
                if (CombatManager.Instance.IsInProgress)
                {
                    if (cardPlay.Card.Type == CardType.Skill)
                    {
                        DynamicVars["SkillsPlayedThisTurn"].BaseValue++;
                        if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 2 == 0)
                        {
                            StartPulsing();
                        }

                        if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 3 == 0)
                        {
                            Flash();
                            
                            var sacCards = Array.Empty<CardModel>();
                            for (int i = 0; i < Amount; i++)
                            {
                                var sacCard = CombatState.CreateCard<MinionSacrifice>(Owner.Player);
                                CardCmd.Upgrade(sacCard);
                                CardCmd.Enchant<Steady>(sacCard, 1);
                                sacCards.AddItem(sacCard);
                            }
                            
                            await CardPileCmd.AddGeneratedCardsToCombat(sacCards, PileType.Hand, Owner.Player);
                            
                            await PowerCmd.Remove(this);
                        }
                    }
                }
            }
        }
}