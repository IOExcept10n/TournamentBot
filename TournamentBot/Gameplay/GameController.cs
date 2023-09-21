using System.Text;

namespace TournamentBot.Gameplay
{
    internal class GameController : IGameController
    {
        private readonly List<IGame> sources = new();
        private Game? game;
        private IPlayer? player1;
        private IPlayer? player2;

        public int Round { get; }

        public bool IsLast { get; }

        public bool IsEnded => game?.IsEnded == true;

        public IPlayer? Winner => game!.Winner;

        public IPlayer? Loser => game!.Loser;

        public ITournament Tournament { get; }

        public event EventHandler<GameResultsPair> GameEnded = delegate { };

        IPlayer IGame.Player1 => player1!;

        IPlayer IGame.Player2 => player2!;

        public GameController(ITournament tournament, int round, bool isLast)
        {
            Tournament = tournament;
            Round = round;
            IsLast = isLast;
        }

        public IGameController AddSource(IGame source, GameResult selectRule)
        {
            if (selectRule == GameResult.Lose)
            {
                source.GameEnded += (s, e) =>
                {
                    AddPlayer(e.Loser);
                };
            }
            else
            {
                source.GameEnded += (s, e) =>
                {
                    AddPlayer(e.Winner);
                };
            }
            sources.Add(source);
            return this;
        }

        public void SetWinner(IPlayer winner)
        {
            game!.SetWinner(winner);
        }

        public void Update()
        {
            if (IsEnded) return;
            game?.Update();
            sources.ForEach(x => x.Update());
        }

        private void AddPlayer(IPlayer? player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (player1 == null)
            {
                player1 = player;
            }
            else
            {
                player2 = player;
                game = new(Tournament, player1, player2, Round, IsLast);
                Tournament.RegisterGame(game);
                game.GameEnded += GameEnded;
            }
        }

        public void DrawTree(StringBuilder output)
        {
            output.AppendLine(ToString());
            for (int i = 0; i < sources.Count; i++)
            {
                IGame? source = sources[i];
                output.AppendJoin("-", Enumerable.Repeat("-", Tournament.Rounds - Round + 1));
                //if (source.IsEnded &&
                //    (source.Player1 == player1 ||
                //     source.Player2 == player1 ||
                //     source.Player1 == player2 ||
                //     source.Player2 == player2))
                //    output.Append('+');
                source.DrawTree(output);
                if (i != sources.Count - 1)
                    output.AppendLine();
            }
        }

        public override string ToString()
        {
            return game?.ToString() ?? $"{Round}: Waiting for the next game results.";
        }
    }
}