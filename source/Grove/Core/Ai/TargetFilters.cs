﻿namespace Grove.Core.Ai
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Details.Mana;
  using Targeting;

  public static class TargetFilters
  {
    public delegate IEnumerable<ITarget> InputSelectorDelegate(TargetsCandidates candidates);

    public delegate IEnumerable<Targets> OutputSelectorDelegate(IEnumerable<ITarget> targets);

    public static TargetsFilterDelegate PermanentsByDescendingScore(Controller controller = Ai.Controller.Opponent)
    {
      return p =>
        {
          var candidates = p.Candidates();

          switch (controller)
          {
            case Ai.Controller.Opponent:
              candidates = candidates
                .RestrictController(p.Opponent);
              break;
            case Ai.Controller.SpellOwner:
              candidates = candidates
                .RestrictController(p.Controller);
              break;
          }

          var targets = p.Targets(candidates
            .OrderByDescending(x => x.Card().Score));

          if (targets.Count == 0 && p.ForceOne)
          {
            targets = p.Targets(p.Candidates().OrderBy(x => x.Card().Score));
          }

          return targets;
        };
    }

    public static TargetsFilterDelegate Opponent()
    {
      return p =>
        {
          return p.Targets(p.Candidates()
            .Where(x => x.Player() == p.Opponent));
        };
    }

    public static TargetsFilterDelegate PumpAttackerOrBlocker(int? power, int? thougness)
    {
      return p =>
        {
          power = power ?? p.MaxX;
          thougness = thougness ?? p.MaxX;
          var candidates = GetCandidatesPumpAttackerOrBlocker(power, thougness, p);
          return p.Targets(candidates);
        };
    }

    private static IEnumerable<Card> GetCandidatesPumpAttackerOrBlocker(int? power, int? thougness,
                                                                        TargetFilterParameters p)
    {
      return p.Candidates().RestrictController(p.Controller)
        .Select(
          x =>
            new
              {
                Card = x.Card(),
                Gain = p.Combat.CalculateGainIfGivenABoost(x.Card(), power.Value, thougness.Value)
              })
        .Where(x => x.Gain > 0)
        .OrderByDescending(x => x.Gain)
        .Select(x => x.Card);
    }

    public static TargetsFilterDelegate CounterSpell()
    {
      return p =>
        {
          var candidates = p.Candidates().RestrictController(p.Opponent)
            .Take(1);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate CombatEquipment()
    {
      return p =>
        {
          if (p.Step == Step.FirstMain)
          {
            return p.Targets(p.Candidates()
              .Where(x => x.Card().CanAttack)
              .Select(x => new
                {
                  Card = x.Card(),
                  Score = CalculateAttackerScore(x.Card(), p.Combat)
                })
              .OrderByDescending(x => x.Score)
              .Where(x => x.Score > 0)
              .Select(x => x.Card));
          }

          return p.Targets(p.Candidates()
            .Where(x => x.Card().CanBlock())
            .Select(x => new
              {
                Card = x.Card(),
                Score = CalculateBlockerScore(x.Card(), p.Combat)
              })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 0)
            .Select(x => x.Card));
        };
    }

    public static TargetsFilterDelegate DealDamage(Func<TargetFilterParameters, int> amount)
    {
      return p => DealDamage(amount(p))(p);
    }
    
    public static TargetsFilterDelegate DealDamage(int? amount = null)
    {
      return p =>
        {
          amount = amount ?? p.MaxX;
                              
          var candidates = p.Candidates()
            .Where(x => x == p.Opponent)
            .Select(x => new
              {
                Target = x,
                Score = ScoreCalculator.CalculateLifelossScore(x.Player().Life, amount.Value)
              })
            .Concat(
              p.Candidates()
                .Where(x => x.IsCard() && x.Card().Controller == p.Opponent)
                .Select(x => new
                  {
                    Target = x,
                    Score = x.Card().LifepointsLeft <= amount ? x.Card().Score : 0
                  }))
            .OrderByDescending(x => x.Score)
            .Select(x => x.Target);

          var targets = p.Targets(candidates);

          if (p.ForceOne && targets.Count == 0)
          {
            targets = p.Targets(p.Candidates().OrderByDescending(x => x.Card().Toughness));
          }

          return targets;
        };
    }

    private static int CalculateAttackerScore(Card card, Combat combat)
    {
      return combat.CouldBeBlockedByAny(card) ? 5 : 0 + card.Power.Value;
    }

    private static int CalculateBlockerScore(Card card, Combat combat)
    {
      var count = combat.CountHowManyThisCouldBlock(card);

      if (count > 0)
      {
        return count*10 + card.Toughness.Value;
      }

      return 0;
    }

    public static TargetsFilterDelegate CombatEnchantment()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Controller)
            .Where(x => x.Card().CanAttack)
            .Select(x => new
              {
                Card = x.Card(),
                Score = CalculateAttackerScore(x.Card(), p.Combat)
              })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 0)
            .Select(x => x.Card);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate ShieldHexproof()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Controller)
            .Where(x => p.Stack.CanBeDestroyedByTopSpell(x.Card(), targetOnly: true))
            .OrderByDescending(x => x.Card().Score);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate ShieldIndestructible()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Controller)
            .Where(x => p.Stack.CanBeDestroyedByTopSpell(x.Card()) || p.Combat.CanBeDealtLeathalCombatDamage(x.Card()))
            .OrderByDescending(x => x.Card().Score);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate Destroy()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Opponent)
            .OrderByDescending(x => x.Card().Score);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate Any(params TargetsFilterDelegate[] delegates)
    {
      return p =>
        {
          var result = new List<Targets>();
          result.AddRange(delegates[0](p));

          for (var i = 1; i < delegates.Length; i++)
          {
            var filterDelegate = delegates[i];
            var targetsList = filterDelegate(p);
            foreach (var targets in targetsList)
            {
              if (result.Contains(targets))
                continue;

              result.Add(targets);
            }
          }

          return result;
        };
    }

    public static TargetsFilterDelegate ReduceToughness(int? amount = null)
    {
      return p =>
        {
          amount = amount ?? p.MaxX;

          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Opponent)
            .Select(x => new
              {
                Target = x,
                Score = x.Card().LifepointsLeft <= amount ? x.Card().Score : 0
              })
            .OrderByDescending(x => x.Score)
            .Select(x => x.Target);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate IncreasePowerAndToughness(int? power, int? toughness)
    {
      return p =>
        {
          var candidates =
            GetCandidatesPumpAttackerOrBlocker(power, toughness, p).Concat(
              p.Candidates()
                .Where(x => x.Card().Controller == p.Controller)
                .Where(x => p.Stack.CanBeDealtLeathalDamageByTopSpell(x.Card()))
              );

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate CostTap()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .OrderBy(x => x.Card().Score);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate CostSacrificeGainLife()
    {
      return p =>
        {
          if (p.Stack.CanTopSpellReducePlayersLifeToZero(p.Controller))
          {
            return p.Targets(p.Candidates()
              .OrderBy(x => x.Card().Score));
          }

          var candidates = p.Candidates()
            .Where(x => p.Stack.CanBeDestroyedByTopSpell(x.Card()) ||
              p.Combat.CanBeDealtLeathalCombatDamage(x.Card()))
            .Take(1);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate Bounce()
    {
      return Destroy();
    }

    public static TargetsFilterDelegate Controller()
    {
      return p =>
        {
          return p.Targets(p.Candidates()
            .Where(x => x.Player() == p.Controller));
        };
    }

    public static TargetsFilterDelegate Exile()
    {
      return Destroy();
    }

    public static TargetsFilterDelegate TapCreature()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Where(x => x.Card().Controller == p.Opponent)
            .OrderByDescending(x => x.Card().Power);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate CreatureWithGreatestPower()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .OrderByDescending(x => x.Card().Power)
            .Take(1);

          return p.Targets(candidates);
        };
    }

    public static TargetsFilterDelegate PreventDamageFromSourceToAnyTarget()
    {
      return p =>
        {
          var targetPicks = new List<ITarget>();
          var sourcePicks = new List<ITarget>();

          if (!p.Stack.IsEmpty && p.Stack.TopSpell is IDamageDealing)
          {
            var damageToPlayer = p.Stack.GetDamageTopSpellWillDealToPlayer(p.Controller);

            if (damageToPlayer > 0)
            {
              targetPicks.Add(p.Controller);
              sourcePicks.Add(p.Stack.TopSpell);
            }

            var creatureKilledByTopSpell = p.Candidates(1)
              .Where(x => x.IsCard())
              .Where(x => p.Stack.CanBeDealtLeathalDamageByTopSpell(x.Card()))
              .OrderByDescending(x => x.Card().Score)
              .FirstOrDefault();

            if (creatureKilledByTopSpell != null)
            {
              targetPicks.Add(creatureKilledByTopSpell);
              sourcePicks.Add(p.Stack.TopSpell);
            }
          }

          if (p.Step == Step.DeclareBlockers)
          {
            if (!p.Controller.IsActive)
            {
              var attacker = p.Combat.GetAttackerWhichWillDealGreatestDamageToDefender();

              if (attacker != null)
              {
                targetPicks.Add(p.Controller);
                sourcePicks.Add(attacker);
              }
            }

            var creatureKilledInCombat = p.Candidates(1)
              .Where(x => x.IsCard())
              .Select(x => new
                {
                  Target = x.Card(),
                  Source = p.Combat.GetBestDamagePreventionCandidateForAttackerOrBlocker(x.Card())
                })
              .Where(x => x.Source != null)
              .OrderByDescending(x => x.Target.Score)
              .FirstOrDefault();

            if (creatureKilledInCombat != null)
            {
              targetPicks.Add(creatureKilledInCombat.Target);
              sourcePicks.Add(creatureKilledInCombat.Source);
            }
          }

          return p.MultipleTargets(sourcePicks, targetPicks);
        };
    }

    public static TargetsFilterDelegate DamageRedirection()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .Select(x => new
              {
                Card = x.Card(),
                Score = CalculateDamageRedirectionScore(x.Card(), p)
              })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Card);

          return p.Targets(candidates);
        };
    }

    private static int CalculateDamageRedirectionScore(Card card, TargetFilterParameters p)
    {
      const int protectionOffset = 200;

      if (card.Controller == p.Opponent)
      {
        return card.Score;
      }

      if (card.HasProtectionFrom(ManaColors.Red | ManaColors.Black))
      {
        return protectionOffset + card.Score;
      }

      return card.Toughness.Value - 3;
    }

    public static TargetsFilterDelegate PreventDamageFromSourceToController()
    {
      return p =>
        {
          var targetPicks = new List<ITarget>();

          if (!p.Stack.IsEmpty && p.Stack.TopSpell is IDamageDealing && p.Candidates().Contains(p.Stack.TopSpell))
          {
            var damageToPlayer = p.Stack.GetDamageTopSpellWillDealToPlayer(p.Controller);

            if (damageToPlayer > 0)
            {
              targetPicks.Add(p.Stack.TopSpell);
            }
          }

          if (p.Step == Step.DeclareBlockers)
          {
            if (!p.Controller.IsActive)
            {
              var attacker = p.Combat.GetAttackerWhichWillDealGreatestDamageToDefender(
                card => p.Candidates().Contains(card));

              if (attacker != null)
              {
                targetPicks.Add(attacker);
              }
            }
          }

          return p.Targets(targetPicks);
        };
    }

    public static TargetsFilterDelegate GreatestConvertedManaCost()
    {
      return p =>
        {
          var candidates = p.Candidates()
            .OrderBy(x => x.Card().ManaCost.Converted)
            .Take(1);

          return p.Targets(candidates);
        };
    }
  }
}