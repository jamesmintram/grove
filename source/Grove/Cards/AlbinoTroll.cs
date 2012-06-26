﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;
  using Core.Costs;
  using Core.Effects;

  public class AlbinoTroll : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Albino Troll")
        .ManaCost("{1}{G}")
        .Type("Creature Troll")
        .Text(
          "{Echo} {1}{G} (At the beginning of your upkeep, if this came under your control since the beginning of your last upkeep, sacrifice it unless you pay its echo cost.){EOL}{1}{G}: Regenerate Albino Troll.")
        .Power(3)
        .Toughness(3)
        .Echo("{1}{G}")
        .Abilities(
          C.ActivatedAbility(
            "{1}{G}: Regenerate Albino Troll.",
            C.Cost<TapOwnerPayMana>((c, _) => c.Amount = "{1}{G}".ParseManaAmount()),
            C.Effect<Regenerate>(),
            timing: Timings.RegenerateThis(), 
            category: EffectCategories.Protector));        
    }
  }
}