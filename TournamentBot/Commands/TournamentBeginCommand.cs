using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class TournamentBeginCommand : TournamentCommand
    {
        public TournamentBeginCommand() : base("begin")
        {
        }

        protected override bool PerformCommand(long author, string[] arguments, ref ITournament? tournament, StringBuilder output)
        {
            if (tournament != null)
            {
                output.AppendLine("Sorry, you've already started a tournament.");
                //await client.SendTextMessageAsync(message.Chat.Id, "Sorry, you've already started a tournament.", cancellationToken: token);
                return false;
            }
            else
            {
                var players = arguments.Skip(1);
                if (!players.Any())
                {
                    output.AppendLine("Can't create a tournament without players. Please, try again.");
                    return false;
                }
                if (players.Distinct().Count() < players.Count())
                {
                    output.AppendLine("Can't create a tournament because you have more than one player with the same names. Check and try again.");
                    return false;
                }
                if (int.TryParse(players.First(), out int key))
                {
                    players = players.Skip(1);
                }
                else
                {
                    key = new Random().Next();
                }
                tournament = new Tournament(key, players);
                tournament.Ended += (s, e) =>
                {
                    UserControlSystem.UserTournaments.TryRemove(author, out _);
                };
                if (UserControlSystem.UserTournaments.TryAdd(author, tournament))
                {
                    output.AppendLine("The tournament was successfully created!");
                    output.AppendLine($"The tournament shuffle key: `{key}`");
                    return true;
                }
                else
                {
                    output.AppendLine("Sorry, please try adding the tournament again.");
                    //await client.SendTextMessageAsync(message.Chat.Id, "Sorry, please try add the tournament again.", cancellationToken: token);
                    return false;
                }
            }
        }
    }
}
