using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TournamentBot.Commands;
using TournamentBot.Extensions;
using TournamentBot.Gameplay;

namespace TournamentBot
{
    internal class Program
    {
        private static TelegramBotClient _client = null!;

        public static async Task Main()
        {
            string token = System.IO.File.ReadAllText("token.txt");
            _client = new(token);
            CommandManager.Commands = new()
            {
                new StartCommand(),
                new TournamentBeginCommand(),
                new TournamentWinCommand(),
                new TournamentEndCommand(),
                new TournamentGamesCommand(),
                new LeaderboardCommand(),
                new TreeCommand()
            };
            using CancellationTokenSource cts = new();
            ReceiverOptions options = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            _client.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync, receiverOptions: options, cancellationToken: cts.Token);
            var me = await _client.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            // Cancel the bot after the program completion.
            cts.Cancel();
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var message = update.Message!;
                        if (message.Text?.StartsWith('/') == true)
                        {
                            int index = message.Text.Contains(' ') ? message.Text.IndexOf(' ') : message.Text.Length;
                            string commandName = message.Text[1..index];
                            if (CommandManager.SupportsCommand(commandName))
                            {
                                await CommandManager.HandleCommandAsync(commandName, client, message, token);
                            }
                        }
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        try
                        {
                            var query = update.CallbackQuery!;
                            string name = query.Data ?? "";
                            long author = query.From.Id;
                            UserControlSystem.UserTournaments.TryGetValue(author, out var tournament);
                            StringBuilder output = new();
                            if (tournament != null)
                            {
                                tournament.RegisterWin(name);
                                output.AppendLine($"Player {name} won successfully.");
                                TournamentCommandHelpers.FillInfo(tournament, output);
                                await client.EditMessageTextAsync(chatId: query.Message!.Chat.Id,
                                                                  messageId: query.Message!.MessageId,
                                                                  text: output.ToString(),
                                                                  replyMarkup: TournamentCommandHelpers.CreateKeyboardMarkup(tournament),
                                                                  cancellationToken: token);
                            }
                            else
                            {
                                output.AppendLine("Sorry, can't set the player win in the current state..");
                                if (tournament != null) TournamentCommandHelpers.FillInfo(tournament, output);
                                await client.EditMessageTextAsync(chatId: query.Message!.Chat.Id,
                                                                  messageId: query.Message!.MessageId,
                                                                  text: output.ToString(),
                                                                  replyMarkup: tournament != null ? TournamentCommandHelpers.CreateKeyboardMarkup(tournament) : null,
                                                                  cancellationToken: token);
                            }
                        }
                        catch (Exception ex)
                        {
                            await client.SendTextMessageAsync(
                                chatId: update.CallbackQuery!.Message!.Chat.Id,
                                cancellationToken: token,
                                text: "The error while handling the query. Please, try again."
#if DEBUG
                                + $"\nError details: {ex.Message}"
#endif
                                );
                        }
                        break;
                    }
            }
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}