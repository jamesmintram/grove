﻿namespace Grove.Core.Cards
{
  using System.Collections.Generic;
  using System.Linq;
  using Ai;
  using Effects;
  using Infrastructure;
  using Targeting;
  using Zones;

  [Copyable]
  public abstract class Ability : IEffectSource
  {
    private TargetSelector _targetSelector = TargetSelector.NullSelector;
    public TargetSelector TargetSelector { get { return _targetSelector; } }

    public Player Controller { get { return OwningCard.Controller; } }
    protected IEffectFactory EffectFactory { get; private set; }
    protected Game Game { get; set; }
    protected Stack Stack { get { return Game.Stack; } }
    public CardText Text { get; set; }
    public bool UsesStack { get; set; }
    public Card SourceCard { get; protected set; }
    public EffectCategories EffectCategories { get; set; }
    public Card OwningCard { get; protected set; }
    public abstract int CalculateHash(HashCalculator calc);

    public void EffectWasCountered() {}

    void IEffectSource.EffectWasPushedOnStack() {}
    void IEffectSource.EffectWasResolved() {}

    bool IEffectSource.IsTargetStillValid(ITarget target, bool wasKickerPaid)
    {
      return TargetSelector.IsValidEffectTarget(target);
    }

    public void Effect(IEffectFactory effectFactory)
    {
      EffectFactory = effectFactory;
    }

    public void Targets(IEnumerable<ITargetValidatorFactory> effect, IEnumerable<ITargetValidatorFactory> cost,
      TargetSelectorAiDelegate aiSelector)
    {
      _targetSelector = new TargetSelector(
        effectValidators: effect.Select(x => x.Create(OwningCard, Game)),
        costValidators: cost.Select(x => x.Create(OwningCard, Game)),
        aiSelector: aiSelector
        );
    }

    public override string ToString()
    {
      return string.Format("{0}'s ability", OwningCard);
    }
  }
}