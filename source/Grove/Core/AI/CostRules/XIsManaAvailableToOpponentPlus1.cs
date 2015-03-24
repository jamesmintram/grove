﻿namespace Grove.AI.CostRules
{
  public class XIsManaAvailableToOpponentPlus1 : CostRule
  {
    public override int CalculateX(CostRuleParameters p)
    {
      if (Stack.IsEmpty || Stack.TopSpellOwner == p.Controller)
        return int.MaxValue;

      return p.Controller.Opponent.GetAvailableMana(
        canUseConvoke: p.OwningCard.Has().Convoke,
        canUseDelve: p.OwningCard.Has().Delve) + 1;
    }
  }
}