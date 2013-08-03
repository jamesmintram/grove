﻿namespace Grove.Cards
{
  using System;
  using System.Collections.Generic;
  using Gameplay.Characteristics;
  using Gameplay.Misc;

  public class BloatedToad : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Bloated Toad")
        .ManaCost("{2}{G}")
        .Type("Creature Frog")
        .Text("{Protection from blue}{EOL}Cycling {2} ({2}, Discard this card: Draw a card.)")        
        .Power(2)
        .Toughness(2)
        .Cycling("{2}")
        .Protections(CardColor.Blue);
    }
  }
}