namespace TournamentBot.Gameplay
{
    /// <summary>
    /// Defines a result of the game process.
    /// </summary>
    public enum GameResult
    {
        /// <summary>
        /// The first team has lost the game.
        /// </summary>
        Lose,

        /// <summary>
        /// The first team has won the game.
        /// </summary>
        Win,

        /// <summary>
        /// The game ended with a draw.
        /// </summary>
        Draw
    }

    /// <summary>
    /// Represents a base interface for any tournament entry (e.g. game, player or the tournament).
    /// </summary>
    internal interface ITournamentElement
    {
        /// <summary>
        /// Provides the access to the tournament that is handling the game.
        /// </summary>
        public ITournament Tournament { get; }
    }
}