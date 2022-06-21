using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Net.NetworkInformation;
using PingAddress = System.Net.NetworkInformation.Ping;

namespace Speedberg.SusBot
{
    public class Ping : Command
    {
        [Command("ping",ClientType.Discord)]
        [Help("Pings an address, or this bot if no parameter is given.","ping","ping [ip address]")]
        [Parameter("ip","The ip address to ping.",true)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            string[] keywords = GetKeywords(message.Content).ToArray();

            if(keywords.Length == 1)
            {
                await message.RespondAsync($"Ping! {DiscordGlobal<State,OldState>.Client.MyClient.Ping}");
            } else {
                try
                {
                    string address = keywords[1];
                    PingAddress ping = new PingAddress();
                    PingReply reply;

                    await Task.Run(async () => {
                        reply = ping.Send(address);
                        if(reply.Status == IPStatus.Success)
                        {
                            await message.RespondAsync($"The address '{address}' responded successfully in {reply.RoundtripTime} ms!");
                        } else {
                            await message.RespondAsync($"The address '{address}' responded with a response of {reply.Status.ToString()}.");
                        }
                    });
                } catch(System.Exception e)
                {
                    await message.RespondAsync($"Error: {e.Message}");
                }
            }
        }

        [SlashCommand("ping","Pings the bot")]
        [SlashCommandOption("ip","The ip address to ping.",SlashCommandOption.Type.String,false)]
        public async Task DiscordSlashCommand(DiscordInteraction response)
        {
            DiscordInteractionResponseBuilder SlashResponse = new DiscordInteractionResponseBuilder();

            SlashResponse.Content = $"Ping! {DiscordGlobal<State,OldState>.Client.MyClient.Ping}";

            await response.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, SlashResponse);
        }

        [SlashCommand("ping","Pings the bot")]
        [SlashCommandOption("ip","The ip address to ping.",SlashCommandOption.Type.String,false)]
        public async Task DiscordSlashCommand(DiscordInteraction response, DiscordInteractionDataOption[] options)
        {
            DiscordFollowupMessageBuilder SlashResponse = new DiscordFollowupMessageBuilder();

            if(options.Length == 1)
            {
                await response.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,new DiscordInteractionResponseBuilder()
                {
                    Content = "Pinging address...",
                });

                try
                {
                    string address = (string)options[0].Value;
                    PingAddress ping = new PingAddress();
                    PingReply reply;

                    await Task.Run(async () => {
                        reply = ping.Send(address);
                        if(reply.Status == IPStatus.Success)
                        {
                            SlashResponse.Content = $"The address '{address}' responded successfully in {reply.RoundtripTime} ms!";
                        } else {
                            SlashResponse.Content = $"The address '{address}' responded with a response of {reply.Status.ToString()}.";
                        }
                    });
                } catch(System.Exception e)
                {
                    SlashResponse.Content = $"Error: {e.Message}";
                }
                await response.CreateFollowupMessageAsync(SlashResponse);
                return;
            }
        }

        [Command("ping",ClientType.Revolt)]
        [Help("Pings the bot.","ping")]
        public async Task RevoltCommand()
        {

        }
    }
}