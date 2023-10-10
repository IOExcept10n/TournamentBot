using Telegram.Bot;
using Telegram.Bot.Types;

namespace TournamentBot.Commands
{
    internal class StartCommand : Command
    {
        public StartCommand() : base("start")
        {
        }

        public override async Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token)
        {
            await client.SendTextMessageAsync(message.Chat.Id, @$"
Welcome to the Tournament Bot!
You can use the following commands:
1. /begin [key] (members*) - starts a new tournament and registers the following members. The key is needed to generate the grid.
2. /cancel - cancels a tournament.
3. /games - gets the games history of the current tournament.
4. /tree - draws a tournament tree with all members.
5. /leaderboard - gets the information about all tournament members.
6. /win (winner) - sets a winner (use one of names set in the tournament).", cancellationToken: token);
        }
    }
}