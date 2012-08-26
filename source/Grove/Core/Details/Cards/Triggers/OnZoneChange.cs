﻿namespace Grove.Core.Details.Cards.Triggers
{
  using System;
  using Infrastructure;
  using Messages;
  using Zones;

  public class OnZoneChange : Trigger, IReceive<CardChangedZone>
  {
    public Func<TriggeredAbility, Card, bool> Filter =
      (ability, card) => ability.OwningCard == card;

    public Zone From { get; set; }
    public Zone To { get; set; }    

    public void Receive(CardChangedZone message)
    {            
      if (!Filter(Ability, message.Card))
        return;

      if (message.From == Zone.None)
        return;

      if (From == Zone.None && To == message.To)
        Set(message);
      else if (From == message.From && To == message.To)
        Set(message);
      else if (From == message.From && To == Zone.None)
        Set(message);
    }
  }
}