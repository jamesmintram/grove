﻿namespace Grove
{
  using System;
  using System.Collections.Generic;
  using Grove.AI;
  using Grove.Decisions;
  using Grove.Events;
  using Grove.Infrastructure;

  [Copyable]
  public abstract class GameObject
  {
    public bool IsInitialized { get { return Game != null; } }
    protected Game Game { get; set; }
    protected Players Players { get { return Game.Players; } }
    protected Stack Stack { get { return Game.Stack; } }
    protected Combat Combat { get { return Game.Combat; } }
    protected TurnInfo Turn { get { return Game.Turn; } }
    protected SearchRunner Ai { get { return Game.Ai; } }            
    protected ChangeTracker ChangeTracker { get { return Game.ChangeTracker; } }

    public void SaveDecisionResult(object result)
    {
      Game.Recorder.SaveDecisionResult(result);
    }

    public List<ITarget> GenerateTargets(Func<Zone, Player, bool> zoneFilter)
    {
      var targets = new List<ITarget>();

      Players.Player1.GetTargets(zoneFilter, targets);
      Players.Player2.GetTargets(zoneFilter, targets);
      Stack.GenerateTargets(zoneFilter, targets);

      return targets;
    }

    protected int GenerateRandomNumber(int minValue, int maxValue)
    {
      return Game.Random.Next(minValue, maxValue);
    }

    protected IList<int> GetRandomPermutation(int start, int count)
    {
      return Game.Random.GetRandomPermutation(start, count);
    }

    protected bool FlipACoin(Player who)
    {
      // in search always consider winning the coin flip
      var hasWon = Ai.IsSearchInProgress || Game.Random.FlipACoin();
      Publish(new PlayerFlippedCoinEvent(who, hasWon));

      return hasWon;
    }

    protected int RollADice()
    {
      return Game.Random.RollADice();
    }

    protected void Publish<T>(T message)
    {
      Game.Publish(message);
    }

    protected void Unsubscribe(object obj = null)
    {
      obj = obj ?? this;
      Game.Unsubscribe(obj);
    }

    protected void Subscribe(object obj = null)
    {
      obj = obj ?? this;
      Game.Subscribe(obj);
    }

    protected void Enqueue(Decision decision)
    {
      Game.Enqueue(decision);
    }
  }
}