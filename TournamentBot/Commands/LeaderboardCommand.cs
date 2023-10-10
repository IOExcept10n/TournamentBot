using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentBot.Extensions;
using TournamentBot.Gameplay;

namespace TournamentBot.Commands
{
    internal class LeaderboardCommand : TournamentCommand
    {
        public LeaderboardCommand() : base("leaderboard", "top")
        {
        }

        protected override bool PerformCommand(long author, string[] arguments, ref ITournament? tournament, StringBuilder output)
        {
            if (tournament != null)
            {
                TournamentCommandHelpers.FillLeaderboard(tournament, output);
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
