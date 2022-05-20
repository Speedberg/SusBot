namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;

    using DSharpPlus.Entities;

    public class SecretAmogus : Command
    {
        [Command("sus",ClientType.Discord)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            await message.RespondAsync("ඞ");
        }
    }
}