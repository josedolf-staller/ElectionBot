﻿using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static ElectionBot.DatabaseManager;

namespace ElectionBot.Modules.ElectionRunner
{
    public class SaveElection : ModuleBase<SocketCommandContext>
    {
        [Command("save-election")]
        [Alias("saveelection")]
        [RequireOwner]
        public async Task SaveElectionAsync(string type)
        {
            List<string> adminTypes = new List<string>()
            {
                "admin",
                "a",
                "administrator"
            };
            bool isAdmin = adminTypes.Contains(type.ToLower());

            await SaveAsync(Context.Guild, isAdmin);
            await Context.Channel.SendMessageAsync("Voter IDs and Keys have been saved to `voters.csv` files.");
        }

        public static async Task SaveAsync(SocketGuild g, bool isAdmin)
        {
            int i = 0;
            int j = 1;
            string path = $"voters{j}-{(isAdmin ? "a" : "m")}-{g.Id}.csv";

            File.WriteAllText(path, "name,voter_identifier,voter_key,email,vote_weight");
            foreach ((SocketGuildUser user, int voterKey, int weight) in await electionDatabase.Voters.GetVotersAsync(g, isAdmin))
            {
                if (i >= 20)
                {
                    j++;
                    path = $"voters{j}-{(isAdmin ? "a" : "m")}-{g.Id}.csv";
                    File.WriteAllText(path, "name,voter_identifier,voter_key,email,vote_weight");

                    i = 0;
                }

                string name = user.Nickname ?? user.Username;
                name = name.Contains(",") ? $"\"{name}\"" : name;

                string username = user.Username;
                username = username.Contains(",") ? $"\"{username}\"" : username;

                File.AppendAllText(path, $"\n{name},{username},{voterKey},,{weight}");
                i++;
            }
        }
    }
}