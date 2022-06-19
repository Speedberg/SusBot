using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Speedberg.SusBot.Modules.Utility
{
    public class Server : Command
    {
        [Command("server",ClientType.Discord)]
        [Help("Displays information about the current server.",
        "server",
        "server [server ID]")]
        [Parameter("Server ID","The ID of a public server to fetch.",true)]
        public async Task DiscordCommand(DiscordMessage message, DiscordGuild commandGuild)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            string[] keywords = GetKeywords(message.Content).ToArray();
            if(keywords.Length == 2)
            {
                ulong guildID = 0;
                if(!ulong.TryParse(keywords[1],out guildID))
                {
                    await message.RespondAsync("Error: incorrect server ID");
                    return;
                }
                try
                {
                    var previewGuild = await DiscordGlobal<State,OldState>.Client.MyClient.GetGuildPreviewAsync(guildID);
                    embed.WithTitle("Server Info");
                    embed.WithDescription("Displaying info for an external server.");
                    embed.AddField("ID",previewGuild.Id.ToString() ?? "N/A");
                    embed.AddField("Name",previewGuild.Name ?? "N/A");
                    embed.AddField("Display Icon", ($"https://cdn.discordapp.com/icons/{previewGuild.Id}/{previewGuild.Icon}.png") ?? "N/A");
                    embed.AddField("Description",previewGuild.Description ?? "N/A");
                    embed.AddField("Splash",($"https://cdn.discordapp.com/splashes/{previewGuild.Id}/{previewGuild.Splash}.png") ?? "N/A");
                    embed.AddField("Discovery Splash",($"https://cdn.discordapp.com/discovery-splashes/{previewGuild.Id}/{previewGuild.DiscoverySplash}.png") ?? "N/A");
                    embed.AddField("Creation Data", $"<t:{previewGuild.CreationTimestamp.ToUnixTimeSeconds().ToString()}:R>" ?? "N/A");
                    embed.AddField("Emoji Count", $"{previewGuild.Emojis.Count.ToString()} emojis" ?? "N/A");
                    string emojis = "";
                    foreach(var emoji in previewGuild.Emojis)
                    {
                        emojis += " " + (emoji.Value.Name ?? "Unknown Emoji") + " ";
                    }
                    if(emojis.Length > 1024) emojis = emojis.Substring(0,1020) + "...";
                    embed.AddField("Emojis", emojis ?? "N/A");
                    embed.AddField("Members",$"{previewGuild.ApproximatePresenceCount} online / {previewGuild.ApproximateMemberCount}");
                } catch(System.Exception e)
                {
                    Debug.Error($"{e}");
                    await message.RespondAsync("Error: the server is not public");
                    return;
                }
            } else {
                var guild = await DiscordGlobal<State,OldState>.Client.MyClient.GetGuildAsync(commandGuild.Id,true); 
                //Members
                var members = await guild.GetAllMembersAsync();
                int memberCount = members.Count;
                int botCount = 0;
                foreach(var member in members)
                {
                    if(member.IsBot) botCount += 1;
                }

                //Channels
                var channels = await guild.GetChannelsAsync();
                int textChannelCount = 0;
                int voiceChannelCount = 0;
                int categoryCount = 0;
                int privateChannels = 0;
                foreach(var channel in channels)
                {
                    if(channel.Type.HasFlag(DSharpPlus.ChannelType.Private)) privateChannels += 1;
                    if(channel.IsCategory || channel.Type.HasFlag(DSharpPlus.ChannelType.Category)) categoryCount += 1;
                    if(channel.Type.HasFlag(DSharpPlus.ChannelType.Text) && !channel.Type.HasFlag(DSharpPlus.ChannelType.Category) && !channel.IsCategory
                    && !channel.Type.HasFlag(DSharpPlus.ChannelType.Voice)) textChannelCount += 1;
                    if(channel.Type.HasFlag(DSharpPlus.ChannelType.Voice)) voiceChannelCount += 1;
                }

                var me = await guild.GetMemberAsync(DiscordGlobal<State,OldState>.Client.MyClient.CurrentUser.Id);
                embed.WithTitle("Server Info");
                embed.WithDescription("Displaying info for the current server.");
                embed.AddField("ID",guild.Id.ToString() ?? "N/A");
                embed.AddField("Name",guild.Name ?? "N/A");
                embed.AddField("Display Icon",guild.IconUrl ?? "N/A");
                embed.AddField("Description",guild.Description ?? "N/A");
                embed.AddField("Banner",guild.BannerUrl ?? "N/A");
                embed.AddField("Owner", GetOwner(guild) ?? "N/A");
                embed.AddField("Creation Data",guild.CreationTimestamp.ToString() ?? "N/A");
                embed.AddField("Channels",$"{textChannelCount + voiceChannelCount} channels - {textChannelCount} text channels, {voiceChannelCount} voice channels, {categoryCount} categories, {privateChannels} private channels detected");
                embed.AddField("Members", $"{guild.ApproximatePresenceCount} online members out of {memberCount} including {botCount} bot(s) - maximum members possible: {guild.MaxMembers}" ?? "N/A");
                embed.AddField("Roles",$"{guild.Roles.Count} roles" ?? "N/A");
                embed.AddField("Emojis", $"{guild.Emojis.Count} emojis" ?? "N/A");
                embed.AddField("Nitro Level", $"{guild.PremiumSubscriptionCount} members boosted - Nitro Tier {guild.PremiumTier.ToString()}" ?? "N/A");
                embed.AddField("Nsfw Level",guild.NsfwLevel.ToString() ?? "N/A");
                embed.AddField("Verification Level",guild.VerificationLevel.ToString() ?? "N/A");
                embed.AddField("Authentication Level",guild.MfaLevel.ToString() ?? "N/A");
                string perms = "";
                foreach(DSharpPlus.Permissions permission in DSharpPlus.Permissions.GetValues(typeof(DSharpPlus.Permissions)))
                {
                    if(me == null)
                    {
                        perms = "None (L)";
                        break;
                    }
                    if (me.Permissions.HasFlag(permission)) perms += (permission.ToString() + " | ");
                }
                embed.AddField("Permissions", perms ?? "N/A");  
            }
            embed.WithFooter(BotGlobal.WebsiteBots);
            await message.RespondAsync(embed.Build());
        }

        private string GetOwner(DiscordGuild guild)
        {
            try
            {
                return guild.Owner.Mention;
            } catch
            {
                return null;
            }
        }
    }
}