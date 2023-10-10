using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class TreeCommand : TournamentCommand
    {
        public TreeCommand() : base("tree", "grid")
        {
            
        }

        protected override bool PerformCommand(long author, string[] arguments, ref ITournament? tournament, StringBuilder output)
        {
            if (tournament != null)
            {
                output.AppendLine("The tournament tree: ");
                tournament.DrawTree(output);
                output.AppendLine();
                return true;
            }
            else
            {
                output.Append("Can't get the tournament info.");
                return false;
            }
        }
    }
}
