﻿namespace Grove
{
  using System.Collections.Generic;
  using Grove.Infrastructure;
  using Modifiers;

  public class ContiniousEffects : GameObject, IAcceptsCardModifier, IAcceptsPlayerModifier
  {
    private readonly TrackableList<ContinuousEffect> _continiousEffects;

    public ContiniousEffects() { _continiousEffects = new TrackableList<ContinuousEffect>(); }

    public ContiniousEffects(IEnumerable<ContinuousEffect> continuousEffects) { _continiousEffects = new TrackableList<ContinuousEffect>(continuousEffects); }

    public void Accept(ICardModifier modifier) { modifier.Apply(this); }

    public void Accept(IPlayerModifier modifier) { modifier.Apply(this); }

    public void Initialize(Card source, Game game)
    {
      Game = game;

      _continiousEffects.Initialize(ChangeTracker);

      foreach (var continiousEffect in _continiousEffects)
      {
        continiousEffect.Initialize(source, Game);
      }
    }

    public void Add(ContinuousEffect effect) { _continiousEffects.Add(effect); }

    public void Remove(ContinuousEffect effect)
    {
      _continiousEffects.Remove(effect);
      effect.Deactivate();
    }
  }
}