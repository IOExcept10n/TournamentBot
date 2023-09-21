namespace TournamentBot.Gameplay
{
    /// <summary>
    /// Represents an element that controls the game process and allows building the tournament tree.
    /// </summary>
    internal interface IGameController : IGame
    {
        /// <summary>
        /// Adds a game results of which will be used to bring players to the next round.
        /// </summary>
        /// <param name="source">The game to add handlers to.</param>
        /// <param name="selectRule">Rule by which to bring players into a round.</param>
        public IGameController AddSource(IGame source, GameResult selectRule);
    }
}