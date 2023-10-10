using Telegram.Bot;
using Telegram.Bot.Types;

namespace TournamentBot
{
    internal interface ICommand
    {
        public string Name { get; }
        public HashSet<string> Aliases { get; }

        public Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token);
    }

    internal abstract class Command : ICommand
    {
        public string Name { get; }

        public HashSet<string> Aliases { get; }

        public Command(string name, IEnumerable<string>? aliases = null)
        {
            Name = name;
            if (aliases == null)
                Aliases = new();
            else
            {
                Aliases = new(aliases);
            }
        }

        public abstract Task HandleAsync(ITelegramBotClient client, Message message, CancellationToken token);
    }

    internal static class CommandManager
    {
        public static List<ICommand> Commands { get; set; } = new();

        public static bool SupportsCommand(string commandName) => Commands.Any(x => x.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));

        public static Task HandleCommandAsync(string command, ITelegramBotClient client, Message message, CancellationToken token)
        {
            return Commands.First(x => x.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase) || x.Aliases.Contains(command)).HandleAsync(client, message, token);
        }
    }
}