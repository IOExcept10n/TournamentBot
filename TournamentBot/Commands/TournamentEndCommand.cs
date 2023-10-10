using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class TournamentEndCommand : TournamentCommand
    {
        public TournamentEndCommand() : base("cancel", "end")
        {
        }

        protected override bool PerformCommand(long author, string[] arguments, ref ITournament? tournament, StringBuilder output)
        {
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
            return false;
        }
    }
}
