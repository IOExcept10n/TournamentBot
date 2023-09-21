namespace TournamentBot.Gameplay
{
    internal readonly struct GameResultsPair
    {
        public readonly IPlayer Winner;
        public readonly IPlayer Loser;

        public GameResultsPair(IPlayer winner, IPlayer loser)
        {
            Winner = winner;
            Loser = loser;
        }
    }
}