namespace TournamentBot.Gameplay
{
    /// <summary>
    /// Represents an interface for the player of the tournament.
    /// </summary>
    internal interface IPlayer : ITournamentElement
    {
        /// <summary>
        /// Gets the nameof the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the ordinal number of a player in the tournament.
        /// </summary>
        /// <remarks>
        /// This property doesn't mean the place of the player, this is just draw number.
        /// </remarks>
        public int Number { get; set; }

        /// <summary>
        /// Gets the value that determines whether the player lose the game technically.
        /// </summary>
        public bool TechnicalLose { get; }

        /// <summary>
        /// Provides the access to the game that the player is currently playing or <see langword="null"/> if player is free for now.
        /// </summary>
        public IGame? Game { get; set; }

        /// <summary>
        /// Handles the end of a game in which player got the following result.
        /// </summary>
        /// <param name="result">The result of a player.</param>
        public void OnGetResult(GameResult result);
    }
}