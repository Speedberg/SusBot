using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Speedberg.SusBot
{
    public class Info : Command
    {
        [Command("info",ClientType.Discord)]
        [Help("Displays info about this bot.")]
        public async Task DiscordCommand(DiscordMessage message)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTitle("Info");
            embed.WithAuthor("SusBot", BotGlobal.Website, "https://cdn.discordapp.com/avatars/734735424571965451/e1575a6efc0f4d85498e8515064368ea.webp?size=240");
            embed.WithDescription($"Discord bot created by <@579729416314421268>.\nType `{DiscordGlobal<State,OldState>.Client.Prefix}help` for a list of commands.\n[Website]({BotGlobal.Website})\n[Invite Link]({BotGlobal.DiscordBotInvite})");
            embed.AddField("Version", Global.Version.ToString(), true);
            await message.RespondAsync(embed.Build());
        }
    }
}