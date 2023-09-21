using TournamentBot.Gameplay;

namespace TournamentBot.Extensions
{
    internal static class Utils
    {
        public static void Win(this IPlayer player) => player.Game?.SetWinner(player);
    }
}