using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TournamentBot.Gameplay;

namespace TournamentBot.Extensions
{
    internal static class TournamentCommandHelpers
    {
        internal static InlineKeyboardMarkup CreateKeyboardMarkup(ITournament tournament)
        {
            List<List<InlineKeyboardButton>> buttons = new();
            if (!tournament.IsEnded)
                foreach (var game in tournament.Games)
                {
                    List<InlineKeyboardButton> insideButtons = new();
                    if (!game.Player1.TechnicalLose)
                    {
                        insideButtons.Add(InlineKeyboardButton.WithCallbackData(game.Player1.Name));
                    }
                    if (!game.Player2.TechnicalLose)
                    {
                        insideButtons.Add(InlineKeyboardButton.WithCallbackData(game.Player2.Name));
                    }
                    buttons.Add(insideButtons);
                }
            return new InlineKeyboardMarkup(buttons);
        }

        internal static void FillInfo(ITournament tournament, StringBuilder builder)
        {
            builder.AppendLine("The tournament state:");
            if (tournament.IsEnded)
            {
                builder.Append("Ended. ");
                FillLeaderboard(tournament, builder);
                return;
            }
            else if (tournament.Games.Any())
            {
                builder.AppendLine("Running. Games:");
                foreach (var game in tournament.Games)
                {
                    builder.AppendLine(game.ToString());
                }
            }
        }

        internal static void FillLeaderboard(ITournament tournament, StringBuilder builder)
        {
            builder.AppendLine("Leaderboard:");
            int position = 0;
            foreach (var place in tournament.Leaderboard ?? Enumerable.Empty<Player>())
            {
                // Create a string in the format "<#>. <name>(<wins>): <games>;"
                builder.Append(++position)
                       .Append('.')
                       .Append(' ')
                       .Append(place?.ToString() ?? "-")
                       .Append('(')
                       .Append(place?.GameResults.Count(x => x == GameResult.Win) ?? 0)
                       .Append(')')
                       .Append(':');
                for (int i = 0; i < place?.GameResults.Count(); i++)
                {
                    var result = place.GameResults.ElementAt(i);
                    switch (result)
                    {
                        case GameResult.Lose:
                            builder.Append(" L"); break;
                        case GameResult.Draw:
                            builder.Append(" D"); break;
                        case GameResult.Win:
                            builder.Append(" W"); break;
                    }
                }
                builder.AppendLine(";");
            }
        }

        internal static void FillGames(ITournament tournament, StringBuilder builder)
        {
            builder.AppendLine("Current games: ");
            foreach (var game in tournament.Games)
            {
                builder.AppendLine(game.ToString());
            }
            builder.AppendLine("Games history: ");
            foreach (var game in tournament.History)
            {
                builder.AppendLine(game.ToString());
            }
        }
    }
}