﻿using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace ElectionBot.Modules.ElectionRunner
{
    public class ClearElections : ModuleBase<SocketCommandContext>
    {
        [Command("clear-elections")]
        [Alias("clearelections")]
        [RequireOwner]
        public async Task ClearElectionsAsync()
        {
            await Task.WhenAll
            (
                DeleteFilesAsync(),
                ClearElection.RemoveElectionAsync(Context.Guild, true),
                ClearElection.RemoveElectionAsync(Context.Guild, false)
            );
            await Context.Channel.SendMessageAsync("All elections have been cleared.");
        }

        private async Task DeleteFilesAsync()
        {
            await Task.Yield();
            int i = 1;
            while (File.Exists($"voters{i}-a-{Context.Guild.Id}.csv"))
            {
                File.Delete($"voters{i}-a-{Context.Guild.Id}.csv");
                i++;
            }
            while (File.Exists($"voters{i}-m-{Context.Guild.Id}.csv"))
            {
                File.Delete($"voters{i}-m-{Context.Guild.Id}.csv");
                i++;
            }
        }
    }
}
