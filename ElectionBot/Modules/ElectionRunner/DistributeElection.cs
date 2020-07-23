﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectionBot.Modules.ElectionRunner
{
    public class DistributeElection : ModuleBase<SocketCommandContext>
    {
        [Command("dist-election")]
        [Alias("distelection", "distribute-election")]
        [RequireOwner]
        public async Task DistributeElectionAsync(string type)
        {
            List<string> adminTypes = new List<string>()
            {
                "admin",
                "a",
                "administrator"
            };
            bool isAdmin = adminTypes.Contains(type.ToLower());
            type = isAdmin ? "Administrator" : "Moderator";

            OverwritePermissions bot = new OverwritePermissions().Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);
            OverwritePermissions allow = new OverwritePermissions().Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
            OverwritePermissions deny = new OverwritePermissions().Modify(viewChannel: PermValue.Deny);

            int i = 20;
            int j = 1;
            ulong id = 0;
            foreach ((SocketGuildUser user, int voterKey, int weight) in await SaveElection.GetVotersAsync(Context.Guild, isAdmin))
            {
                if (i >= 20)
                {
                    id = Context.Guild.CategoryChannels.FirstOrDefault(x => x.Name.ToLower() == $"voter group {j}")?.Id
                        ?? (await Context.Guild.CreateCategoryChannelAsync($"Voter Group {j}", x => x.Position = j + 4)).Id;

                    j++;
                    i = 0;
                }

                ITextChannel channel = Context.Guild.TextChannels.FirstOrDefault(x => x.Name == user.Discriminator && x.CategoryId == id);
                if (channel == null)
                {
                    channel = await Context.Guild.CreateTextChannelAsync(user.Discriminator, x => x.CategoryId = id);
                    await channel.AddPermissionOverwriteAsync(Context.Guild.CurrentUser, bot);
                    await channel.AddPermissionOverwriteAsync(user, allow);
                    await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, deny);
                }

                await channel.SendMessageAsync($"__Voter Info for the Upcoming UCD {type} Election__\n" +
                    $"Voter ID: {user.Username}\n" +
                    $"Voter Key: {voterKey}");

                i++;
            }

            await Context.Channel.SendMessageAsync("Voter IDs have been distributed.");
        }
    }
}
