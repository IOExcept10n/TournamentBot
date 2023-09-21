using System.Text;

namespace TournamentBot.Gameplay
{
    internal class Game : IGame
    {
        public int Round { get; }

        public bool IsLast { get; }

        public bool IsEnded { get; private set; }

        public IPlayer? Winner { get; private set; }

        public IPlayer? Loser { get; private set; }

        public ITournament Tournament { get; }

        public event EventHandler<GameResultsPair> GameEnded = delegate { };

        public IPlayer Player1 { get; }

        public IPlayer Player2 { get; }

        public Game(ITournament tournament, IPlayer player1, IPlayer player2, int round, bool isLast)
        {
            Tournament = tournament;
            Player1 = player1;
            Player2 = player2;
            Player1.Game = this;
            Player2.Game = this;
            Round = round;
            IsLast = isLast;
        }

        public void SetWinner(IPlayer winner)
        {
            if (winner == Player1)
            {
                Player1.OnGetResult(GameResult.Win);
                Winner = Player1;
                Player2.OnGetResult(GameResult.Lose);
                Loser = Player2;
            }
            else if (winner == Player2)
            {
                Player1.OnGetResult(GameResult.Lose);
                Loser = Player1;
                Player2.OnGetResult(GameResult.Win);
                Winner = Player2;
            }
            else return;
            IsEnded = true;
            if (IsLast) Tournament.RegisterLose(Loser);
            GameEnded?.Invoke(this, new(Winner, Loser));
            if (Player1.Game == this) Player1.Game = null;
            if (Player2.Game == this) Player2.Game = null;
        }

        public void Update()
        {
            if (!IsEnded)
            {
                if (Player1.TechnicalLose) SetWinner(Player2);
                else if (Player2.TechnicalLose) SetWinner(Player1);
            }
        }

        public void DrawTree(StringBuilder builder)
        {
            builder.Append(ToString());
        }

        public override string ToString()
        {
            return (IsLast ? "#" : "") + Round + ": " + (IsEnded ? $"({Winner} > {Loser})" : $"{Player1} vs. {Player2}");
        }
    }
}