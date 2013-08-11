﻿namespace Grove.Gameplay.Effects
{
  using Counters;

  public class RemoveAllCountersFromOwner : Effect
  {
    private readonly CounterType _counterType;

    private RemoveAllCountersFromOwner() {}

    public RemoveAllCountersFromOwner(CounterType counterType)
    {
      _counterType = counterType;
    }

    protected override void ResolveEffect()
    {
      Source.OwningCard.RemoveCounters(_counterType);
    }
  }
}