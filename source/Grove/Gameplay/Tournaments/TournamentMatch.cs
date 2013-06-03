﻿namespace Grove.Gameplay.Tournaments
{
  public class TournamentMatch
  {
    public TournamentMatch(TournamentPlayer player1, TournamentPlayer player2)
    {
      Player1 = player1;
      Player2 = player2;
    }

    public TournamentPlayer Player1 { get; private set; }
    public TournamentPlayer Player2 { get; private set; }

    public TournamentPlayer HumanPlayer { get { return Player1.IsHuman ? Player1 : Player2; } }
    public TournamentPlayer NonHumanPlayer { get { return Player1.IsHuman ? Player2 : Player1; } }

    public bool IsSimulated { get { return !Player1.IsHuman && !Player2.IsHuman; } }
  }
}