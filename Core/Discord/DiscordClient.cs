using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Net.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Speedberg.Bots.Core.Discord
{
    public class Client : BotClient
    {
        public DiscordClient DiscordClient;

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
                
                DiscordClient.MessageCreated += OnMessageCreated;

                await DiscordClient.ConnectAsync(activity: activity);

                DiscordClient.Ready += async (client, args) =>
                {
                    DiscordGlobal.CachedStateGuild = await DiscordClient.GetGuildAsync(DiscordGlobal.StateServerID);
                    if(DiscordGlobal.CachedStateGuild == null)
                    {
                        Console.WriteLine("[ERROR] Could not find State Guild.");
                        return;
                    }

                    DiscordGlobal.CachedStateChannel = await DiscordClient.GetChannelAsync(DiscordGlobal.StateChannelID);
                    if(DiscordGlobal.CachedStateChannel == null || DiscordGlobal.CachedStateChannel.GuildId != DiscordGlobal.StateServerID)
                    {
                        Console.WriteLine("[ERROR] Could not find State Channel.");
                        return;
                    }

                    //Download current state
                    State currentState = await FetchState(DiscordGlobal.CachedStateChannel.LastMessageId);

                    if(currentState == null)
                    {
                        Console.WriteLine("Creating new state...");
                        currentState = new State();
                        currentState.startedAt = Global.StartTime;
                        currentState.instanceID = -1;
                    }

                    //Update state values
                    currentState.instanceID += 1;
                    currentState.uuid = Guid.NewGuid().ToString();
                    DiscordGlobal.BotState = currentState;
                    Console.WriteLine("Bot UUID: {0} Instance: {1}", currentState.uuid,currentState.instanceID);

                    //Upload new state
                    await SaveState("Startup");
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
            if(eventArgs.Author.IsBot && eventArgs.Author.Id == DiscordGlobal.BotID)
            {
                await DetectStateChange(eventArgs.Channel, eventArgs.Message);
                return;
            }

            if(eventArgs.Message.Content.ToLower().Contains("egg") && !eventArgs.Author.IsBot)
            {
                await eventArgs.Message.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient,":egg:",false));
            }
            if((System.DateTime.Now.ToBinary() % 7) == 0)
            {
                await eventArgs.Channel.SendMessageAsync(DiscordEmoji.FromName(DiscordClient,":egg:"));
            }

            await CheckForCommand(eventArgs.Message,eventArgs.Guild,eventArgs.Channel);
        }

        private async Task DetectStateChange(DiscordChannel channel, DiscordMessage message)
        {
            if(channel.GuildId != DiscordGlobal.StateServerID) return;
            if(channel.Id != DiscordGlobal.StateChannelID) return;

            State fileState = await FetchState(message.Id);

            if(fileState == null) return;
            if(fileState.uuid == DiscordGlobal.BotState.uuid) return;

            //New instance detected - dispose this client gracefully
            if(fileState.instanceID > DiscordGlobal.BotState.instanceID)
            {
                Console.WriteLine("death");

                Global.Cts.Cancel();
            } else
            {
                //Old state values - copy these
                fileState.uuid = DiscordGlobal.BotState.uuid;
                fileState.instanceID = DiscordGlobal.BotState.instanceID;

                DiscordGlobal.BotState = fileState;
                Console.WriteLine("Loaded old state!");
            }
        }

        private async Task SaveState(string type)
        {
            try
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();

                string json = JsonConvert.SerializeObject(DiscordGlobal.BotState);
                byte[] byteArray = Encoding.ASCII.GetBytes(json);

                using(MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    builder.WithContent(type);
                    builder.WithFile("State.json", memoryStream);
                    Console.WriteLine("Saving state!");
                    await DiscordClient.SendMessageAsync(DiscordGlobal.CachedStateChannel, builder);
                }
            } catch(Exception e)
            {
                Console.WriteLine("[ERROR]: {0}", e);
            }
        }

        private async Task<State> FetchState(ulong? messageID)
        {
            byte[] data = null;

            if (messageID == null) return null;

            try
            {
                DiscordMessage lastState = await DiscordGlobal.CachedStateChannel.GetMessageAsync((ulong)messageID);
                DiscordAttachment file = lastState.Attachments[0];
                HttpClient _client = new HttpClient();

                data = await _client.GetByteArrayAsync(file.Url);
                string json = System.Text.Encoding.Default.GetString(data);
                Console.WriteLine("JSON: {0}", json);

                return JsonConvert.DeserializeObject<State>(json);
            } catch (System.Exception e)
            {
                //The message was probably deleted
                return null;
            }
        }
    
        public async Task Dispose()
        {
            //Save state on shutdown
            await SaveState("Shutdown");
            Console.WriteLine("Disconnecting and disposing...");
            DiscordClient.Dispose();
        }
    }
}