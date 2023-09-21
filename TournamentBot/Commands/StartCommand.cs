﻿using Telegram.Bot;
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
1. /tournament - a command for the tournament management. Available modes:
* start (members*) - starts a new tournament and registers the following members.
* end - cancels a tournament.
* games - gets the games history of the current tournament.
* tree - draws a tournament tree with all members.
* leaderboard - gets the information about all tournament members.
4. /win (winner) - sets a winner (use one of names set in the tournament).", cancellationToken: token);
        }
    }
}