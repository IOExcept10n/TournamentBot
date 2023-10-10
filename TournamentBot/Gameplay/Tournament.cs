using System.Text;
using TournamentBot.Extensions;

namespace TournamentBot.Gameplay
{
    internal class Tournament : ITournament
    {
        private readonly Dictionary<string, Player> playerNames;
        private readonly List<IPlayer> allPlayers;
        private readonly List<IPlayer> remainPlayers;
        private readonly List<Player> leaderboard;
        private readonly List<IGame> games;
        private readonly List<IGame> history;
        private bool leaderboardReady;

        public IEnumerable<IPlayer> Players => allPlayers;

        public IEnumerable<Player?>? Leaderboard
        {
            get
            {
                return leaderboardReady ?
                    leaderboard :
                    Enumerable.Repeat<Player?>(null, allPlayers.Count - leaderboard.Count).Concat(Enumerable.Reverse(leaderboard));
            }
        }

        public IGame TournamentRoot { get; }

        public bool IsEnded { get; private set; }

        public IEnumerable<IGame> Games => games;

        public IEnumerable<IGame> History => history;

        public int Rounds { get; }

        public event EventHandler? Ended = delegate { };

        public Tournament(int key, IEnumerable<string> members)
        {
            // Define the lists.
            allPlayers = new();
            playerNames = new();
            leaderboard = new();
            this.games = new();
            history = new();
            // Create players records.
            int amount = 0;
            foreach (var memberName in members)
            {
                var player = new Player(this, memberName);
                allPlayers.Add(player);
                playerNames.Add(memberName, player);
                amount++;
            }
            // Handle the special case when the player is only one:
            if (amount <= 1)
            {
                leaderboard.AddRange(allPlayers.Cast<Player>());
                Cancel();
                remainPlayers = new();
                TournamentRoot = null!;
                return;
            }
            // Add fake players to get the nearest power of two.
            int nearestAmount = (int)((uint)amount).NextPowerOfTwo();
            for (int i = amount; i < nearestAmount; i++)
            {
                var fakePlayer = new FakePlayer(this);
                allPlayers.Add(fakePlayer);
            }
            // Shuffle the players.
            Random rand = new(key);
            remainPlayers = new(allPlayers.OrderBy(x => rand.Next()));
            // Start creating the tournament grid.
            // Stage 1: Define initial games - add players in pairs.
            IGame[] games = new IGame[nearestAmount / 2];
            for (int i = 0; i < games.Length; i++)
            {
                var player1 = remainPlayers[i * 2];
                var player2 = remainPlayers[i * 2 + 1];
                var game = new Game(this, player1, player2, 0, false);
                games[i] = game;
                RegisterGame(game);
            }
            int controllers = games.Length / 2, round = 1;
            // Handle the special case when there are only 2 players.
            if (controllers == 0)
            {
                TournamentRoot = games[0];
                TournamentRoot.GameEnded += HandleLastGame;
                return;
            }
            IGame[] oldGrid1 = new IGame[controllers];
            IGame[] oldGrid2 = new IGame[controllers];
            // Stage 2: Define the initial game controllers
            for (int i = 0; i < controllers; i++)
            {
                var game1 = games[i * 2];
                var game2 = games[i * 2 + 1];
                var controller1 = new GameController(this, round, false)
                    .AddSource(game1, GameResult.Win)
                    .AddSource(game2, GameResult.Win);
                var controller2 = new GameController(this, round, true)
                    .AddSource(game1, GameResult.Lose)
                    .AddSource(game2, GameResult.Lose);
                oldGrid1[i] = controller1;
                oldGrid2[i] = controller2;
            }
            // Stage 3: perform the recursive tree building for other rounds.
            while (controllers > 1)
            {
                round++;
                // 3.1. Add the set of the controllers for the players who lost in the main grid and won in the additional grid.
                IGame[] grid2Mix = new IGame[controllers];
                for (int i = 0; i < controllers; i++)
                {
                    var mainController = oldGrid1[i];
                    var loseController = oldGrid2[i];
                    var newController = new GameController(this, round, true)
                        .AddSource(loseController, GameResult.Win)
                        .AddSource(mainController, GameResult.Lose);
                    grid2Mix[i] = newController;
                }
                oldGrid2 = grid2Mix;
                // 3.2. Decrease the amount of controllers, make the next round.
                round++;
                controllers /= 2;
                IGame[] grid1Row = new IGame[controllers];
                IGame[] grid2Row = new IGame[controllers];
                for (int i = 0; i < controllers; i++)
                {
                    // Make a controller for the win games
                    var game1 = oldGrid1[i * 2];
                    var game2 = oldGrid1[i * 2 + 1];
                    var controller1 = new GameController(this, round, false)
                        .AddSource(game1, GameResult.Win)
                        .AddSource(game2, GameResult.Win);
                    grid1Row[i] = controller1;
                    // Make a controller for the lose grid
                    game1 = oldGrid2[i * 2];
                    game2 = oldGrid2[i * 2 + 1];
                    var controller2 = new GameController(this, round, true)
                        .AddSource(game1, GameResult.Win)
                        .AddSource(game2, GameResult.Win);
                    grid2Row[i] = controller2;
                }
                oldGrid1 = grid1Row;
                oldGrid2 = grid2Row;
            }
            // Stage 4: the last controllers should be correctly ended.
            // Now we should have got only 2 controllers: one in each grid.
            var lastWinController = oldGrid1[0];
            var lastLoseController = oldGrid2[0];
            // Firstly, we make a controller to fight for the third place
            round++;
            var thirdPlaceController = new GameController(this, round, true)
                .AddSource(lastWinController, GameResult.Lose)
                .AddSource(lastLoseController, GameResult.Win);
            round++;
            //thirdPlaceController.GameEnded += HandleThirdPlace;
            // And then make for the first place
            var firstPlaceController = new GameController(this, round, false)
                .AddSource(lastWinController, GameResult.Win)
                .AddSource(thirdPlaceController, GameResult.Win);
            firstPlaceController.GameEnded += HandleLastGame;
            // The end XD
            Rounds = round;
            TournamentRoot = firstPlaceController;
            TournamentRoot.Update();
        }

        public void DrawTree(StringBuilder output) => TournamentRoot.DrawTree(output);

        private void HandleLastGame(object? sender, GameResultsPair e)
        {
            leaderboard.Add((Player)e.Loser!);
            leaderboard.Add((Player)e.Winner!);
            EndTournament();
        }

        private void EndTournament()
        {
            IsEnded = true;
            leaderboard.Reverse();
            leaderboardReady = true;
            Ended?.Invoke(this, EventArgs.Empty);
        }

        public void RegisterLose(IPlayer player)
        {
            if (player is Player p)
            {
                remainPlayers.Remove(p);
                leaderboard.Add(p);
            }
            TournamentRoot.Update();
        }

        public void RegisterWin(string playerName)
        {
            if (!IsEnded && playerNames.TryGetValue(playerName, out var player))
            {
                player.Win();
            }
            TournamentRoot.Update();
        }

        public void Cancel()
        {
            IsEnded = true;
            games.Clear();
            Ended?.Invoke(this, EventArgs.Empty);
        }

        public void RegisterGame(IGame game)
        {
            games.Add(game);
            game.GameEnded += (s, e) =>
            {
                games.Remove(game);
                history.Add(game);
            };
        }
    }
}