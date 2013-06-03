﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Gameplay.Effects;
  using Gameplay.Misc;

  public class SteamBlast : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Steam Blast")
        .ManaCost("{2}{R}")
        .Type("Sorcery")
        .Text(
          "Steam Blast deals 2 damage to each creature and each player.")
        .FlavorText(
          "The viashino knew of the cracked pipes but deliberately left them unmended to bolster the rig's defenses.")
        .Cast(p =>
          {
            p.Effect = () => new DealDamageToCreaturesAndPlayers(
              amountPlayer: 2,
              amountCreature: 2);
          });
    }
  }
}