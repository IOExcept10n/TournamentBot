using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TournamentBot.Extensions;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal abstract class TournamentCommand : Command
    {
        public TournamentCommand(string name, params string[] aliases) : base(name, aliases)
        {
        }

        public override async Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token)
        {
            string[] arguments = message.Text!.Split();
            long author = message.From?.Id ?? 0;
            UserControlSystem.UserTournaments.TryGetValue(author, out var tournament);
            StringBuilder output = new();
            bool result = PerformCommand(author, arguments, ref tournament, output);
            if (output.Length > 0)
            {
                if (tournament != null)
                {
                    var markup = TournamentCommandHelpers.CreateKeyboardMarkup(tournament);
                    if (result) 
                    {
                        TournamentCommandHelpers.FillInfo(tournament, output);
                    }
                    await client.SendTextMessageAsync(message.Chat.Id,
                                                 output.ToString(),
                                                 replyMarkup: markup,
                                                 cancellationToken: token,
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                                                 output.ToString(),
                                                 cancellationToken: token,
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
            }
        }

        protected abstract bool PerformCommand(long author, string[] arguments, ref ITournament? tournament, StringBuilder output);
    }
}