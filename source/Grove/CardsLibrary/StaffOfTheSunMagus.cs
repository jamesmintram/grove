﻿namespace Grove.CardsLibrary
{
    using System.Collections.Generic;
    using Effects;
    using Triggers;

    public class StaffOfTheSunMagus : CardTemplateSource
    {
        public override IEnumerable<CardTemplate> GetCards()
        {
            yield return Card
                .Named("Staff of the Sun Magus")
                .ManaCost("{3}")
                .Type("Artifact")
                .Text("Whenever you cast a white spell or a Plains enters the battlefield under your control, you gain 1 life.")
                .FlavorText("A symbol of conviction in uncertain times.")
                .TriggeredAbility(p =>
                {
                    p.Text = "Whenever you cast a white spell or a Plains enters the battlefield under your control, you gain 1 life.";

                    p.Trigger(new OnCastedSpell((ability, card) => card.HasColor(CardColor.White)));
                    p.Trigger(new OnZoneChanged(
                        to: Zone.Battlefield,
                        filter: (card, ability, game) => card.Is().Land && card.Type.Contains("Plains") && card.Controller == ability.OwningCard.Controller
                        ));

                    p.Effect = () => new YouGainLife(1);

                    p.TriggerOnlyIfOwningCardIsInPlay = true;
                });
        }
    }
}