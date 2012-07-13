﻿namespace Grove.Core.Details.Cards.Effects
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Ai;
  using Controllers;
  using Dsl;
  using Infrastructure;
  using Modifiers;
  using Targeting;

  [Copyable]
  public abstract class Effect : ITarget
  {
    public Action<Effect> AfterResolve = delegate { };
    public Action<Effect> BeforeResolve = delegate { };
    private TrackableList<ITarget> _costTargets;
    private TrackableList<ITarget> _targets;
    private object _triggerContext;
    private bool _wasKickerPaid;
    public bool CanBeCountered { get; set; }
    public Player Controller { get { return Source.OwningCard.Controller; } }
    protected Decisions Decisions { get { return Game.Decisions; } }
    protected Game Game { get; set; }
    public Players Players { get { return Game.Players; } }
    public IEffectSource Source { get; set; }
    public int? X { get; private set; }
    public virtual bool AffectsSource { get { return false; } }

    public bool HasTargets { get { return _targets.Count > 0; } }


    public IEnumerable<ITarget> AllTargets
    {
      get
      {
        foreach (var costTarget in _costTargets)
        {
          yield return costTarget;
        }

        foreach (var target in _targets)
        {
          yield return target;
        }
      }
    }

    public int CalculateHash(HashCalculator calc)
    {
      return HashCalculator.Combine(
        GetType().GetHashCode(),
        calc.Calculate(Source),
        HashCalculator.Combine(_targets.Select(calc.Calculate)),
        CanBeCountered.GetHashCode(),
        _wasKickerPaid.GetHashCode(),
        X.GetHashCode());
    }

    public void AddModifier(IModifier modifier)
    {
      // no card needs this yet
    }

    public void RemoveModifier(IModifier modifier)
    {
      // no card needs this yet
    }

    public ITarget Target()
    {
      return _targets.Count == 0 ? null : _targets[0];
    }

    public ITarget CostTarget()
    {
      return _costTargets.Count == 0 ? null : _costTargets[0];
    }

    public ITarget Target(int index)
    {
      return _targets[index];
    }

    public T Ctx<T>()
    {
      return (T) _triggerContext;
    }

    public void EffectWasCountered()
    {
      Source.EffectWasCountered();
    }

    public void EffectWasPushedOnStack()
    {
      Source.EffectWasPushedOnStack();
    }

    public void EffectWasResolved()
    {
      Source.EffectWasResolved();
    }

    public bool HasEffectStillValidTargets()
    {
      return _targets.Count == 0 || Source.AreTargetsStillValid(_targets, _wasKickerPaid);
    }

    public override string ToString()
    {
      return Source.ToString();
    }

    public void Resolve()
    {
      BeforeResolve(this);
      ResolveEffect();
      AfterResolve(this);
    }

    protected abstract void ResolveEffect();

    public bool HasCategory(EffectCategories effectCategories)
    {
      return ((effectCategories & Source.EffectCategories) != EffectCategories.Generic);
    }

    public void AddTargets(IEnumerable<ITarget> targets)
    {
      _targets.AddRange(targets);
    }

    public void AddTarget(ITarget target)
    {
      _targets.Add(target);
    }

    public void AddCostTargets(IEnumerable<ITarget> targets)
    {
      _costTargets.AddRange(targets);
    }

    public bool HasTarget(Card card)
    {
      return _targets.Any(x => x == card);
    }

    [Copyable]
    public class Factory<TEffect> : IEffectFactory where TEffect : Effect, new()
    {
      public Initializer<TEffect> Init = delegate { };
      public Game Game { get; set; }

      public Effect CreateEffect(IEffectSource source, int? x, bool wasKickerPaid, object triggerContext)
      {
        var effect = new TEffect
          {
            Game = Game,
            Source = source,
            _triggerContext = triggerContext,
            _targets = new TrackableList<ITarget>(Game.ChangeTracker),
            _costTargets = new TrackableList<ITarget>(Game.ChangeTracker),
            X = x,
            _wasKickerPaid = wasKickerPaid,
            CanBeCountered = true
          };

        Init(effect, new CardCreationContext(Game));

        return effect;
      }

      public int CalculateHash(HashCalculator calc)
      {
        return HashCalculator.Combine(
          typeof (TEffect).GetHashCode(),
          Init.GetHashCode());
      }
    }
  }
}