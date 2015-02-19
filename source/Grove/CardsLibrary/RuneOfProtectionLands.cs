﻿namespace Grove.CardsLibrary
{
  using System.Collections.Generic;
  using Grove.Costs;
  using Grove.Effects;
  using Grove.AI.TargetingRules;
  using Grove.AI.TimingRules;

  public class RuneOfProtectionLands : CardTemplateSource
  {
    public override IEnumerable<CardTemplate> GetCards()
    {
      yield return Card
        .Named("Rune of Protection: Lands")
        .ManaCost("{1}{W}")
        .Type("Enchantment")
        .Text(
          "{W}: The next time a land source of your choice would deal damage to you this turn, prevent that damage.{EOL}Cycling {2} ({2}, Discard this card: Draw a card.)")
        .Cast(p => p.TimingRule(new OnFirstMain()))
        .Cycling("{2}")
        .ActivatedAbility(p =>
          {
            p.Text =
              "{W}: The next time a land source of your choice would deal damage to you this turn, prevent that damage.";
            p.Cost = new PayMana(Mana.White);
            p.Effect = () => new PreventDamageFromSourceToController();

            p.TargetSelector.AddEffect(trg =>
              {
                trg.Is.Card(c => c.Is().Land).On.BattlefieldOrStack();
                trg.Message = "Select damage source.";
              });

            p.TargetingRule(new EffectPreventDamageFromSourceToController());
          });
    }
  }
}