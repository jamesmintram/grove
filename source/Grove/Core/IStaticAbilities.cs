﻿namespace Grove.Core
{
  public interface IStaticAbilities
  {
    bool Deathtouch { get; }
    bool Defender { get; }
    bool Fear { get; }
    bool Flying { get; }
    bool Haste { get; }
    bool Hexproof { get; }
    bool Indestructible { get; }
    bool Lifelink { get; }
    bool Shroud { get; }
    bool Trample { get; }
    bool Unblockable { get; }
    bool FirstStrike { get; }
    bool DoubleStrike { get; }
    bool Reach { get; }
    bool Vigilance { get; }
    bool Swampwalk { get; }
  }
}