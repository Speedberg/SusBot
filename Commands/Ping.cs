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

        [Core.Commands.SlashCommand("ping","Pings the bot")]
        [Core.Commands.SlashCommandOption("test","This is a test",Core.Commands.SlashCommandOption.Type.String,true)]
        public async Task DiscordSlashCommand(DiscordInteraction response, DiscordInteractionDataOption[] options)
        {
            //TODO: make global builder, stop creating objects!
            await response.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            {
                Content = $"Ping! {options[0].Value}",
            });
        }

        [Core.Commands.Command("ping",ClientType.Revolt)]
        [Core.Commands.Help("Pings the bot.","ping")]
        public async Task RevoltCommand()
        {

        }
    }
}