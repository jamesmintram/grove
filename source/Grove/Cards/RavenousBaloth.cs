﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TargetingRules;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;

  public class RavenousBaloth : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Ravenous Baloth")
        .ManaCost("{2}{G}{G}")
        .Type("Creature - Beast")
        .Text("Sacrifice a Beast: You gain 4 life.")
        .FlavorText(
          "All we know about the Krosan Forest we have learned from those few who made it out alive.{EOL}—Elvish refugee")
        .Power(4)
        .Toughness(4)
        .ActivatedAbility(p =>
          {
            p.Text = "Sacrifice a Beast: You gain 4 life.";
            p.Cost = new Sacrifice();
            p.Effect = () => new ControllerGainsLife(4);
            p.TargetSelector.AddCost(trg => trg.Is.Card(c => c.Is("beast"), ControlledBy.SpellOwner).On.Battlefield());
            p.TargetingRule(new SacrificeToGainLife());
          });
    }
  }
}