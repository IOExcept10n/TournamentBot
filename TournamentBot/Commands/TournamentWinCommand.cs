using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TournamentBot.Extensions;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class TournamentWinCommand : Command
    {
        public TournamentWinCommand() : base("win")
        {
        }

        public override async Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token)
        {
            string[] arguments = message.Text!.Split();
            long author = message.From?.Id ?? 0;
            UserControlSystem.UserTournaments.TryGetValue(author, out var tournament);
            StringBuilder output = new();
            if (arguments.Length == 2 && tournament != null)
            {
                string name = arguments[1];
                tournament.RegisterWin(name);
                output.AppendLine($"Player {name} won successfully.");
                TournamentCommandHelpers.FillInfo(tournament, output);
                await client.SendTextMessageAsync(message.Chat.Id,
                                                  output.ToString(),
                                                  replyMarkup: TournamentCommandHelpers.CreateKeyboardMarkup(tournament),
                                                  cancellationToken: token);
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Sorry, can't set the player win in the current state..", cancellationToken: token);
            }
        }
    }
}