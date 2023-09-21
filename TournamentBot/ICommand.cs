using Telegram.Bot;
using Telegram.Bot.Types;

namespace TournamentBot
{
    internal interface ICommand
    {
        public string Name { get; }

        public Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token);
    }

    internal abstract class Command : ICommand
    {
        public string Name { get; }

        public Command(string name)
        {
            Name = name;
        }

        public abstract Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token);
    }

    internal static class CommandManager
    {
        public static List<ICommand> Commands { get; set; } = new();

        public static bool SupportsCommand(string commandName) => Commands.Any(x => x.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));

        public static Task HandleCommandAsync(string command, ITelegramBotClient client, Message message, CancellationToken token)
        {
            return Commands.First(x => x.Name.Equals(command)).HandleAsync(client, message, token);
        }
    }
}