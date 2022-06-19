using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Speedberg.SusBot.Modules.Utility
{
    public class Verbose : Command
    {
        [Command("verbose",ClientType.Discord)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.WithTitle("Debug Bot Info");
            embed.AddField("Version",$"{Global.Version.ToString()}",true);
            embed.AddField("Debug Mode", $"{Global.DebugMode.ToString()}",true);
            embed.AddField("Instance", $"{DiscordGlobal<SusBot.State, SusBot.OldState>.Client.BotState.instanceID}", true);
            embed.AddField("Start Timestamp", $"{DiscordGlobal<SusBot.State, SusBot.OldState>.InstanceUptimeToDiscordTimestamp}", true);
            embed.AddField("Error Count", $"{Debug.ErrorCount}", true);
            embed.AddField("Shards",$"{DiscordGlobal<SusBot.State,SusBot.OldState>.Client.MyClient.ShardCount}",true);
            embed.AddField("Intents",$"{DiscordGlobal<SusBot.State,SusBot.OldState>.Client.MyClient.Intents.ToString()}",true);
            embed.AddField("Guild Count",$"{DiscordGlobal<SusBot.State,SusBot.OldState>.Client.MyClient.Guilds.Count.ToString()}",true);

            embed.WithFooter(BotGlobal.WebsiteBots);
            await message.RespondAsync(embed.Build());
        }
    }
}