using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Net.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace Speedberg.Bots.Core.Discord
{
    public class Client : BotClient
    {
        public DiscordClient DiscordClient;
        public Dictionary<ulong, Command> SlashCommands
        {
            get;
            private set;
        }

        public Client()
        {
            _clientType = ClientType.Discord;
        }
        
        public async Task<bool> Setup(string token, string prefix, DiscordActivity activity, params Command[] commands)
        {
            try
            {
                DiscordClient = new DSharpPlus.DiscordClient(new DiscordConfiguration()
                {
                    Token = token,
                    TokenType = DSharpPlus.TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged
                });

                this._token = token;
                this._prefix = prefix;
                this._commands = commands;
                this.SlashCommands = new Dictionary<ulong, Command>();
                
                DiscordClient.MessageCreated += OnMessageCreated;
                DiscordClient.InteractionCreated += OnInteractionCreated;

                await DiscordClient.ConnectAsync(activity: activity);

                //TODO: this half work
                DiscordClient.Resumed += async (client, args) =>
                {
                    args.Handled = true;
                    Console.WriteLine("Session outage detected - checking for state changes...");

                    DiscordGlobal.CachedStartupChannel = await DiscordClient.GetChannelAsync(DiscordGlobal.StartupChannelID);

                    if(DiscordGlobal.CachedStartupChannel.LastMessageId == null) return;

                    //Change detcted since
                    if(DiscordGlobal.CachedStartupChannel.LastMessageId != DiscordGlobal.LastStartupMessageID)
                    {
                        DiscordMessage message = await DiscordGlobal.CachedStartupChannel.GetMessageAsync((ulong)DiscordGlobal.CachedStartupChannel.LastMessageId);
                        DiscordGlobal.LastStartupMessageID = (ulong)DiscordGlobal.CachedStartupChannel.LastMessageId;
                        await DetectStateChange(DiscordGlobal.CachedStartupChannel, message);
                    }
                };

                DiscordClient.Ready += async (client, args) =>
                {
                    Console.WriteLine("[DISCORD] Client created successfully | Version: {0} Guilds: {1}",DiscordClient.GatewayVersion,DiscordClient.Guilds.Count);
                    DiscordGlobal.CachedStateGuild = await DiscordClient.GetGuildAsync(DiscordGlobal.StateServerID);
                    if(DiscordGlobal.CachedStateGuild == null)
                    {
                        Console.WriteLine("[ERROR] Could not find State Guild.");
                        return;
                    }

                    DiscordGlobal.CachedStartupChannel = await DiscordClient.GetChannelAsync(DiscordGlobal.StartupChannelID);
                    if(DiscordGlobal.CachedStartupChannel == null || DiscordGlobal.CachedStartupChannel.GuildId != DiscordGlobal.StateServerID)
                    {
                        Console.WriteLine("[ERROR] Could not find Startup Channel.");
                        return;
                    }

                    DiscordGlobal.CachedShutdownChannel = await DiscordClient.GetChannelAsync(DiscordGlobal.ShutdownChannelID);
                    if(DiscordGlobal.CachedShutdownChannel == null || DiscordGlobal.CachedShutdownChannel.GuildId != DiscordGlobal.StateServerID)
                    {
                        Console.WriteLine("[ERROR] Could not find Shutdown Channel.");
                        return;
                    }

                    //Download current state
                    State currentState = await FetchState(true,DiscordGlobal.CachedStartupChannel.LastMessageId);

                    if(currentState == null)
                    {
                        Console.WriteLine("Creating new state...");
                        currentState = new State();
                        currentState.firstTimestamp = Global.StartTime;
                        currentState.instanceID = -1;
                    }

                    //Update state values
                    currentState.instanceID += 1;
                    currentState.uuid = Guid.NewGuid().ToString();
                    DiscordGlobal.BotState = currentState;
                    currentState.instanceStartTime = Global.StartTime;
                    Console.WriteLine("Bot UUID: {0} Instance: {1}", currentState.uuid,currentState.instanceID);

                    //Upload new state
                    await SaveState(true);

                    //Build slash commands
                    SlashCommands = await Commands.CommandExecutor.BuildSlashCommands(DiscordClient,commands);

                    await DiscordClient.UpdateStatusAsync(new DiscordActivity("egg"));
                };

                return true;
            } catch(Exception e)
            {
                Console.WriteLine("[ERROR]: {0}",e);
                return false;
            }
        }

        private async Task CheckForCommand(DiscordMessage message,DiscordGuild guild,DiscordChannel channel)
        {
            if(message.Content.StartsWith(_prefix))
            {
                string messageContent = message.Content.Substring(1);

                string name = messageContent.Split(' ')[0];

                Command command = Commands.CommandExecutor.FetchCommand(name,_commands);
                if(command == null)
                {
                    await message.RespondAsync("[ERROR]: That command does not exist!");
                    return;
                }

                try
                {
                    await Commands.CommandExecutor.ExecuteAsync(command,message);
                    await Commands.CommandExecutor.ExecuteAsync(command,message,channel);
                    await Commands.CommandExecutor.ExecuteAsync(command,message,guild);
                } catch
                {
                    await message.RespondAsync("[ERROR]: unknown");
                    return;
                }
            }
        }

        private async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            eventArgs.Handled = true;

            if(eventArgs.Author.IsBot && eventArgs.Author.Id == DiscordGlobal.BotID)
            {
                await DetectStateChange(eventArgs.Channel, eventArgs.Message);
                return;
            }

            if(eventArgs.Message.Content.ToLower().Contains("egg") && !eventArgs.Author.IsBot)
            {
                await eventArgs.Message.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient,":egg:",false));
                DiscordGlobal.BotState.eggCount += 1;
                try
                {
                    await client.UpdateStatusAsync(new DiscordActivity($"{DiscordGlobal.BotState.eggCount} {DiscordEmoji.FromName(DiscordClient,":egg:",false)}"));
                } catch(System.Exception impostor)
                {
                    //eject
                }
                if((System.DateTime.Now.ToBinary() % 77) == 0)
                {
                    await eventArgs.Channel.SendMessageAsync(DiscordEmoji.FromName(DiscordClient,":egg:"));
                }
            }

            await CheckForCommand(eventArgs.Message,eventArgs.Guild,eventArgs.Channel);
        }

        private async Task OnInteractionCreated(DiscordClient client, InteractionCreateEventArgs eventArgs)
        {
            if(eventArgs.Interaction.Type == InteractionType.ApplicationCommand)
            {
                if(SlashCommands.ContainsKey(eventArgs.Interaction.ApplicationId))
                {
                    await Commands.CommandExecutor.ExecuteSlashAsync(SlashCommands[eventArgs.Interaction.ApplicationId], eventArgs.Interaction);
                    await Commands.CommandExecutor.ExecuteSlashAsync(SlashCommands[eventArgs.Interaction.ApplicationId], eventArgs.Interaction,eventArgs.Interaction.Data.Options.ToArray());
                } else {
                    await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "Error: idiot",
                    });
                }
                eventArgs.Handled = true;
            }
        }

        private async Task DetectStateChange(DiscordChannel channel, DiscordMessage message)
        {
            if(channel.GuildId != DiscordGlobal.StateServerID) return;
            if(channel.Id != DiscordGlobal.StartupChannelID && channel.Id != DiscordGlobal.ShutdownChannelID) return;

            State fileState = await FetchState((channel.Id == DiscordGlobal.StartupChannelID),message.Id);

            if(fileState == null) return;
            if(fileState.uuid == DiscordGlobal.BotState.uuid) return;

            //New instance detected - dispose this client gracefully
            if(fileState.instanceID > DiscordGlobal.BotState.instanceID)
            {
                Console.WriteLine("New instance detected!");
                Console.WriteLine("death");

                Global.Cts.Cancel();
            } else if(fileState.instanceID != DiscordGlobal.BotState.instanceID)
            {
                //Apply this bot's instance and UUID to old changes
                fileState.uuid = DiscordGlobal.BotState.uuid;
                fileState.instanceID = DiscordGlobal.BotState.instanceID;

                DiscordGlobal.BotState = fileState;
                Console.WriteLine("Loaded old state!");
            }
        }

        private async Task SaveState(bool startup)
        {
            try
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();

                string json = JsonConvert.SerializeObject(DiscordGlobal.BotState);
                byte[] byteArray = Encoding.ASCII.GetBytes(json);

                using(MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    builder.WithContent(startup ? $"Startup: Instance {DiscordGlobal.BotState.instanceID}" : $"Shutdown: Instance {DiscordGlobal.BotState.instanceID}");
                    builder.WithFile("State.json", memoryStream);
                    Console.WriteLine("Saving state!");
                    await DiscordClient.SendMessageAsync(startup ? DiscordGlobal.CachedStartupChannel : DiscordGlobal.CachedShutdownChannel, builder);
                }
            } catch(Exception e)
            {
                Console.WriteLine("[ERROR]: {0}", e);
            }
        }

        private async Task<State> FetchState(bool startup, ulong? messageID)
        {
            return null;
            byte[] data = null;

            if (messageID == null)
            {
                Console.WriteLine("[ERROR] Message ID did not exist!");
                return null;
            }

            try
            {
                Console.WriteLine("Fetching state!");
                DiscordMessage lastState = null;
                if (startup)
                {
                    lastState = await DiscordGlobal.CachedStartupChannel.GetMessageAsync((ulong)messageID);
                    DiscordGlobal.LastStartupMessageID = lastState.Id;
                } else {
                    lastState = await DiscordGlobal.CachedShutdownChannel.GetMessageAsync((ulong)messageID);
                }
                DiscordAttachment file = lastState.Attachments[0];
                HttpClient _client = new HttpClient();

                data = await _client.GetByteArrayAsync(file.Url);
                string json = System.Text.Encoding.Default.GetString(data);
                return JsonConvert.DeserializeObject<State>(json);
            } catch (System.Exception e)
            {
                //The message was probably deleted
                Console.WriteLine("SUSUS " + e);
                return null;
            }
        }
    
        public async Task Dispose()
        {
            //Save state on shutdown
            await SaveState(false);
            Console.WriteLine("Disconnecting and disposing...");
            DiscordClient.Dispose();
        }
    }
}