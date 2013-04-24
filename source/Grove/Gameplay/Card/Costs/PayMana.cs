﻿namespace Grove.Core.Costs
{
  using Grove.Core.Mana;
  using Grove.Core.Targeting;

  public class PayMana : Cost
  {
    private readonly IManaAmount _amount;
    private readonly bool _hasX;
    private readonly ManaUsage _manaUsage;
    private readonly bool _supportsRepetitions;    

    private PayMana() {}

    public PayMana(
      IManaAmount amount,
      ManaUsage manaUsage,
      bool hasX = false,      
      bool supportsRepetitions = false)
    {
      _amount = amount;
      _manaUsage = manaUsage;
      _hasX = hasX;
      _supportsRepetitions = supportsRepetitions;      
    }

    public override bool HasX { get { return _hasX; } }

    public override IManaAmount GetManaCost()
    {
      return _amount;
    }

    protected override void CanPay(CanPayResult result)
    {
      if (!Card.Controller.HasMana(_amount, _manaUsage))
        return;

      result.CanPay = true;

      if (_hasX)
      {
        result.MaxX = Card.Controller.GetConvertedMana(_manaUsage) - _amount.Converted;
      }

      if (_supportsRepetitions)
      {
        var count = 1;
        var amount = _amount;

        while (true)
        {
          amount = amount.Add(amount);

          if (!Card.Controller.HasMana(amount, _manaUsage))
          {
            break;
          }

          count++;
        }

        result.MaxRepetitions = count;
      }

      return;
    }

    protected override void Pay(ITarget target, int? x)
    {
      var amount = _amount;

      if (x.HasValue) 
        amount = amount.Add(x.Value.Colorless());
         
      Card.Controller.Consume(amount, _manaUsage);
    }
  }
}