using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TournamentBot.Extensions;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class TournamentCommand : Command
    {
        public TournamentCommand() : base("tournament")
        {
        }

        public override async Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token)
        {
            string[] arguments = message.Text!.Split();
            long author = message.From?.Id ?? 0;
            UserControlSystem.UserTournaments.TryGetValue(author, out var tournament);
            StringBuilder output = new();
            if (arguments.Length > 1)
            {
                switch (arguments[1])
                {
                    case "start":
                        if (tournament != null)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Sorry, you've already started a tournament.", cancellationToken: token);
                            return;
                        }
                        else
                        {
                            tournament = new Tournament(arguments.Skip(2));
                            tournament.Ended += (s, e) =>
                            {
                                UserControlSystem.UserTournaments.TryRemove(author, out _);
                            };
                            if (UserControlSystem.UserTournaments.TryAdd(author, tournament))
                            {
                                output.AppendLine("The tournament was successfully created!");
                            }
                            else
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Sorry, please try add the tournament again.", cancellationToken: token);
                                return;
                            }
                        }
                        break;

                    case "cancel":
                    case "end":
                        if (tournament != null)
                        {
                            tournament.Cancel();
                            UserControlSystem.UserTournaments.TryRemove(author, out _);
                            output.AppendLine("The tournament has been successfully cancelled.");
                        }
                        else
                        {
                            output.Append("Can't get the tournament info.");
                        }
                        break;

                    case "games":
                        if (tournament != null)
                        {
                            TournamentCommandHelpers.FillGames(tournament, output);
                        }
                        else
                        {
                            output.Append("Can't get the tournament info.");
                        }
                        break;

                    case "leaderboard":
                        if (tournament != null)
                        {
                            TournamentCommandHelpers.FillLeaderboard(tournament, output);
                        }
                        else
                        {
                            output.Append("Can't get the tournament info.");
                        }
                        break;

                    case "tree":
                        if (tournament != null)
                        {
                            output.AppendLine("The tournament tree: ");
                            tournament.DrawTree(output);
                            output.AppendLine();
                        }
                        else
                        {
                            output.Append("Can't get the tournament info.");
                        }
                        break;

                    default:
                        {
                            output.AppendLine("Cannot resolve the command you've asked for.");
                            break;
                        }
                }
            }
            if (tournament != null)
            {
                TournamentCommandHelpers.FillInfo(tournament, output);
                await client.SendTextMessageAsync(message.Chat.Id,
                                                  output.ToString(),
                                                  replyMarkup: TournamentCommandHelpers.CreateKeyboardMarkup(tournament),
                                                  cancellationToken: token);
            }
        }
    }
}