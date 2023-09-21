using System.Text;

namespace TournamentBot.Gameplay
{
    /// <summary>
    /// Represents an interface for the game classes.
    /// </summary>
    internal interface IGame : ITournamentElement
    {
        /// <summary>
        /// Gets a value that determines if the game is ended or not.
        /// </summary>
        public bool IsEnded { get; }

        public int Round { get; }

        /// <summary>
        /// Represents a winner of the game.
        /// </summary>
        public IPlayer? Winner { get; }

        /// <summary>
        /// Represents a loser of the game.
        /// </summary>
        public IPlayer? Loser { get; }

        /// <summary>
        /// Gets the first player of the game.
        /// </summary>
        public IPlayer Player1 { get; }

        /// <summary>
        /// Gets the second player of the game.
        /// </summary>
        public IPlayer Player2 { get; }

        /// <summary>
        /// Occurs when the game ends.
        /// </summary>
        public event EventHandler<GameResultsPair> GameEnded;

        /// <summary>
        /// Sets the winner of the game.
        /// </summary>
        /// <param name="winner">The player who won the game.</param>
        public void SetWinner(IPlayer winner);

        /// <summary>
        /// Updates the game state to check if the results are already available.
        /// </summary>
        public void Update();

        /// <summary>
        /// Creates a game tree where a game waits for its results.
        /// </summary>
        /// <returns>A string with the members or sources of the game.</returns>
        public void DrawTree(StringBuilder builder);
    }
}