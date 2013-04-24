﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TargetingRules;
  using Core.Dsl;
  using Core.Effects;
  using Core.Triggers;

  public class SeasonedMarshal : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Seasoned Marshal")
        .ManaCost("{2}{W}{W}")
        .Type("Creature Human Soldier")
        .Text("Whenever Seasoned Marshal attacks, you may tap target creature.")
        .FlavorText("There are only two rules of tactics: never be without a plan, and never rely on it.")
        .Power(2)
        .Toughness(2)
        .TriggeredAbility(p =>
          {
            p.Text = "Whenever Seasoned Marshal attacks, you may tap target creature.";
            p.Trigger(new OnAttack());
            p.Effect = () => new TapTargets();

            p.TargetSelector.AddEffect(trg =>
              {
                trg.Is.Creature().On.Battlefield();
                trg.MinCount = 0;
                trg.MaxCount = 1;
              });

            p.TargetingRule(new TapCreature());
          });
    }
  }
}