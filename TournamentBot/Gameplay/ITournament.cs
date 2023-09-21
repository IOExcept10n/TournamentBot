using System.Text;

namespace TournamentBot.Gameplay
{
    /// <summary>
    /// Represents an interface for the tournament classes.
    /// </summary>
    internal interface ITournament
    {
        /// <summary>
        /// Gets the value that determines whether the tournament has been ended.
        /// </summary>
        public bool IsEnded { get; }

        /// <summary>
        /// All players who are encountered into the tournament.
        /// </summary>
        public IEnumerable<IPlayer> Players { get; }

        /// <summary>
        /// Gets the set of games that are currently running.
        /// </summary>
        public IEnumerable<IGame> Games { get; }

        /// <summary>
        /// Gets the history of the tournament.
        /// </summary>
        public IEnumerable<IGame> History { get; }

        /// <summary>
        /// Gets the total amount of the tournament rounds.
        /// </summary>
        public int Rounds { get; }

        /// <summary>
        /// Provides a leaderboard for the players list.
        /// </summary>
        public IEnumerable<Player?>? Leaderboard { get; }

        /// <summary>
        /// Occurs when the tournament is ended.
        /// </summary>
        public event EventHandler? Ended;

        /// <summary>
        /// Registers a win to a player who won the game.
        /// </summary>
        /// <param name="playerName"></param>
        public void RegisterWin(string playerName);

        /// <summary>
        /// Registers a lose to remove the player from the hierarchy.
        /// </summary>
        /// <param name="player"></param>
        public void RegisterLose(IPlayer player);

        /// <summary>
        /// Cancels the tournament.
        /// </summary>
        public void Cancel();

        /// <summary>
        /// Adds a game info to the tournament. After the ending of the game, it will be removed.
        /// </summary>
        /// <param name="game">The game to place to the <see cref="Games"/> property.</param>
        public void RegisterGame(IGame game);

        public void DrawTree(StringBuilder builder);
    }
}