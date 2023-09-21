using System.Collections.Concurrent;

namespace TournamentBot.Gameplay
{
    internal static class UserControlSystem
    {
        public static ConcurrentDictionary<long, ITournament> UserTournaments { get; } = new();
    }
}