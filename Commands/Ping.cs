namespace Speedberg.Bots
{
    using Core;
    using System.Threading.Tasks;
    using DSharpPlus;
    using DSharpPlus.Entities;

    public class Ping : Command
    {
        [Core.Commands.Command("ping",ClientType.Discord)]
        [Core.Commands.Help("Pings the bot.","ping")]
        public async Task DiscordCommand(DiscordMessage message)
        {
            await message.RespondAsync($"Ping! {DiscordGlobal.Client.DiscordClient.Ping}");
        }

        [Core.Commands.Command("ping",ClientType.Revolt)]
        [Core.Commands.Help("Pings the bot.","ping")]
        public async Task RevoltCommand()
        {

        }
    }
}