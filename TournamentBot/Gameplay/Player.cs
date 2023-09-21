namespace TournamentBot.Gameplay
{
    internal class Player : IPlayer
    {
        protected readonly List<GameResult> results;

        public string Name { get; }

        public int Number { get; set; }

        public bool TechnicalLose { get; protected set; }

        public IGame? Game { get; set; }

        public ITournament Tournament { get; }

        public IEnumerable<GameResult> GameResults => results;

        public Player(ITournament tournament, string name)
        {
            results = new();
            Tournament = tournament;
            Name = name;
        }

        public void OnGetResult(GameResult result)
        {
            results.Add(result);
        }

        public void Disqualify()
        {
            TechnicalLose = true;
        }

        public override string ToString()
        {
            string name = Name;
            if (TechnicalLose) name += " (X)";
            return name;
        }
    }
}