namespace TournamentBot.Gameplay
{
    internal class FakePlayer : Player
    {
        public FakePlayer(ITournament tournament) : base(tournament, "-")
        {
            TechnicalLose = true;
        }

        public override string ToString() => "--";
    }
}