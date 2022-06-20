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
    public class Client<MyState, MyOldState> : BotClient
    where MyState : Core.State, new()
    where MyOldState : Core.State, new()
    {
        public DiscordClient MyClient;

        public ulong BotID { get; private set; }

        public Dictionary<ulong, Command> SlashCommands
        {
            get;
            private set;
        }

        //For storage reasons, it makes sense for this to be public
        public MyState BotState;

        /// <summary>
        /// Fired when the values for <see cref="Speedberg.Bots.Core.Discord.Client{State, OldState}.BotState"/> are modified.
        /// </summary>
        public AsyncEvent OnStateChanged;

        /// <summary>
        /// Fired when the bot is being disposed.
        /// </summary>
        public AsyncEvent OnDispose;

        /// <summary>
        /// Fired when the bot is ready.
        /// </summary>
        public AsyncEvent<ReadyEventArgs> OnReady;

        /// <summary>
        /// Fired before command checks are done.
        /// </summary>
        public AsyncEvent<MessageCreateEventArgs> OnMessageCreateStart;
        /// <summary>
        /// Fired after command checks are done.
        /// </summary>
        public AsyncEvent<MessageCreateEventArgs> OnMessageCreateEnd;

        /// <summary>
        /// Fired after the bot resumes after an outage.
        /// </summary>
        public AsyncEvent<ReadyEventArgs> OnResumed;

        /// <summary>
        /// Cancels command checks for the currently created message.
        /// </summary>
        public bool CancelCommandChecksThisMessage;

        /// <summary>
        /// Determines whether the bot is listening to events.
        /// </summary>
        public bool IsListening;

        public Client()
        {
            _clientType = ClientType.Discord;
            IsListening = false;
        }
        
        public async Task<bool> Setup(string token, string prefix, DiscordActivity activity, params Command[] commands)
        {
            IsListening = false;

            Debug.Log("Setup started...");

            try
            {
                MyClient = new DSharpPlus.DiscordClient(new DiscordConfiguration()
                {
                    Token = token,
                    TokenType = DSharpPlus.TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged
                });

                this._token = token;
                this._prefix = prefix;
                this._commands = commands;
                this.SlashCommands = new Dictionary<ulong, Command>();
                
                MyClient.MessageCreated += OnMessageCreated;
                MyClient.InteractionCreated += OnInteractionCreated;

                await MyClient.ConnectAsync(activity: activity);

                MyClient.Resumed += async (client, args) =>
                {
                    args.Handled = true;
                    Debug.Warn("Session outage detected - checking for state changes...");

                    DiscordGlobal<MyState,MyOldState>.CachedStartupChannel = await MyClient.GetChannelAsync(DiscordGlobal<MyState,MyOldState>.StartupChannelID);

                    if(DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.LastMessageId == null) return;

                    //Change detcted since outage - TODO: this half works
                    if(DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.LastMessageId != DiscordGlobal<MyState,MyOldState>.LastStartupMessageID)
                    {
                        Debug.Warn("Session outage detected new state!");
                        DiscordMessage message = await DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.GetMessageAsync((ulong)DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.LastMessageId);
                        DiscordGlobal<MyState,MyOldState>.LastStartupMessageID = (ulong)DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.LastMessageId;
                        await DetectStateChange(DiscordGlobal<MyState,MyOldState>.CachedStartupChannel, message);
                    }

                    if(OnResumed != null) await OnResumed.Invoke(args);
                };

                MyClient.Ready += async (client, args) =>
                {
                    args.Handled = true;

                    BotID = MyClient.CurrentUser.Id;

                    Debug.Log($"Client created successfully | ID: {BotID} Version: {MyClient.GatewayVersion} Guilds: {MyClient.Guilds.Count}");

                    DiscordGlobal<MyState,MyOldState>.CachedStateGuild = await MyClient.GetGuildAsync(DiscordGlobal<MyState,MyOldState>.StateServerID);
                    if(DiscordGlobal<MyState,MyOldState>.CachedStateGuild == null)
                    {
                        Debug.Error("Startup failed. Reason: Could not find State Guild.");
                        return;
                    }

                    DiscordGlobal<MyState,MyOldState>.CachedStartupChannel = await MyClient.GetChannelAsync(DiscordGlobal<MyState,MyOldState>.StartupChannelID);
                    if(DiscordGlobal<MyState,MyOldState>.CachedStartupChannel == null || DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.GuildId != DiscordGlobal<MyState,MyOldState>.StateServerID)
                    {
                        Debug.Error("Startup failed. Reason: Could not find Startup Channel.");
                        return;
                    }

                    DiscordGlobal<MyState,MyOldState>.CachedShutdownChannel = await MyClient.GetChannelAsync(DiscordGlobal<MyState,MyOldState>.ShutdownChannelID);
                    if(DiscordGlobal<MyState,MyOldState>.CachedShutdownChannel == null || DiscordGlobal<MyState,MyOldState>.CachedShutdownChannel.GuildId != DiscordGlobal<MyState,MyOldState>.StateServerID)
                    {
                        Debug.Error("Startup failed. Reason: Could not find Shutdown Channel.");
                        return;
                    }

                    //Download current state
                    Debug.Log("Fetching last startup state...");
                    MyState currentState = await FetchState(true,DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.LastMessageId);

                    if(currentState == null)
                    {
                        Debug.Log("No states found - creating a new one!");
                        currentState = new MyState();
                        currentState.firstTimestamp = Global.StartTime;
                        currentState.instanceID = 0;
                    }

                    //Update old state values
                    currentState.instanceID += 1;
                    currentState.uuid = Guid.NewGuid().ToString();

                    BotState = currentState;

                    //Set new state values
                    currentState.instanceStartTime = Global.StartTime;

                    Debug.Log($"Bot UUID: {currentState.uuid} Instance: {currentState.instanceID}");

                    //Upload new state
                    Debug.Log("Uploading new startup state...");
                    await SaveState(true);

                    //Build slash commands
                    Debug.Log("Building slash commands...");
                    SlashCommands = await Commands.CommandExecutor.BuildSlashCommands(MyClient,commands);

                    if(OnStateChanged != null) await OnStateChanged.Invoke();

                    IsListening = true;

                    if(OnReady != null) await OnReady.Invoke(args);
                };

                Debug.Log("Setup successful!");
                return true;
            } catch(Exception e)
            {
                Debug.Error($"Startup failed. Reason: {e}");
                return false;
            }
        }

        private async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            eventArgs.Handled = true;

            if(eventArgs.Author.IsBot && eventArgs.Author.Id == BotID)
            {
                await DetectStateChange(eventArgs.Channel, eventArgs.Message);
                return;
            }

            if(!IsListening) return;

            if(OnMessageCreateStart != null) await OnMessageCreateStart.Invoke(eventArgs);
            if(!CancelCommandChecksThisMessage) await CheckForCommand(eventArgs.Message,eventArgs.Guild,eventArgs.Channel);
            if(OnMessageCreateEnd != null) await OnMessageCreateEnd.Invoke(eventArgs);
        }

        private async Task OnInteractionCreated(DiscordClient client, InteractionCreateEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            if(!IsListening) return;

            if(eventArgs.Interaction.Type == InteractionType.ApplicationCommand)
            {
                if(SlashCommands.ContainsKey(eventArgs.Interaction.ApplicationId))
                {
                    await Commands.CommandExecutor.ExecuteSlashAsync(SlashCommands[eventArgs.Interaction.ApplicationId], eventArgs.Interaction,eventArgs.Interaction.Data?.Options?.ToArray() ?? null);
                    await Commands.CommandExecutor.ExecuteSlashAsync(SlashCommands[eventArgs.Interaction.ApplicationId], eventArgs.Interaction);
                } else {
                    await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "Error: idiot",
                    });
                }
            }
        }

        private async Task CheckForCommand(DiscordMessage message,DiscordGuild guild,DiscordChannel channel)
        {
            if(!IsListening) return;

            if(message.Content.StartsWith(_prefix))
            {
                string messageContent = message.Content.Substring(1);

                string name = messageContent.Split(' ')[0];

                Command command = Commands.CommandExecutor.FetchCommand(name,_commands);

                if(command == null)
                {
                    await message.RespondAsync("That command does not exist!");
                    return;
                }

                try
                {
                    await Commands.CommandExecutor.ExecuteAsync(command,message);
                    await Commands.CommandExecutor.ExecuteAsync(command,message,channel);
                    await Commands.CommandExecutor.ExecuteAsync(command,message,guild);
                } catch
                {
                    Debug.Warn($"A command with the following name failed to execute: {name}");
                    return;
                }
            }
        }

        private async Task DetectStateChange(DiscordChannel channel, DiscordMessage message)
        {
            Debug.Log("Testing for state change...");
            if(channel.GuildId != DiscordGlobal<MyState,MyOldState>.StateServerID) return;
            if(channel.Id != DiscordGlobal<MyState,MyOldState>.StartupChannelID && channel.Id != DiscordGlobal<MyState,MyOldState>.ShutdownChannelID) return;

            MyState fileState = await FetchState((channel.Id == DiscordGlobal<MyState,MyOldState>.StartupChannelID),message.Id);

            if(fileState == null) return;
            if(fileState.uuid == BotState.uuid) return;

            Debug.Log("State change detcetd!");

            //New instance detected - dispose this client gracefully
            if(fileState.instanceID > BotState.instanceID)
            {
                Debug.Log("New instance detected!");
                
                //death
                Global.Cts.Cancel();
            }

            //Only load new data if from shutdown - startup should only be used to detect a new client
            else if(fileState.instanceID != BotState.instanceID && channel.Id == DiscordGlobal<MyState,MyOldState>.ShutdownChannelID)
            {
                //Apply this bot's instance and UUID to old changes
                fileState.uuid = BotState.uuid;
                fileState.instanceID = BotState.instanceID;

                BotState = fileState;
                Debug.Log("Loaded old state!");

                if(OnStateChanged != null) await OnStateChanged.Invoke();
            }
        }

        private async Task SaveState(bool startup)
        {
            //Prevents unwanted saving in debug mode
            if(Global.DebugMode) return;

            Debug.Log("Attempting to save bot state...");
            try
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();

                string json = JsonConvert.SerializeObject(BotState);
                byte[] byteArray = Encoding.ASCII.GetBytes(json);

                using(MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    builder.WithContent(startup ? $"Startup: Instance {BotState.instanceID}" : $"Shutdown: Instance {BotState.instanceID}");
                    builder.WithFile("State.json", memoryStream);
                    await MyClient.SendMessageAsync(startup ? DiscordGlobal<MyState,MyOldState>.CachedStartupChannel : DiscordGlobal<MyState,MyOldState>.CachedShutdownChannel, builder);
                }
                Debug.Log("Bot state saved successfully!");
            } catch(Exception e)
            {
                Debug.Error($"Failed to save state. Reason: {0}");
            }
        }

        private async Task<MyState> FetchState(bool startup, ulong? messageID)
        {
            Debug.Log($"Attempting to fetch state... Startup: {startup}");
            byte[] data = null;

            if (messageID == null)
            {
                Debug.Error("Failed to fetch state. Reason: Message ID did not exist!");
                return null;
            }

            try
            {
                DiscordMessage lastState = null;
                if(startup)
                {
                    lastState = await DiscordGlobal<MyState,MyOldState>.CachedStartupChannel.GetMessageAsync((ulong)messageID);
                    //?????????
                    DiscordGlobal<MyState,MyOldState>.LastStartupMessageID = lastState.Id;
                } else {
                    lastState = await DiscordGlobal<MyState,MyOldState>.CachedShutdownChannel.GetMessageAsync((ulong)messageID);
                }
                DiscordAttachment file = lastState.Attachments[0];
                HttpClient _client = new HttpClient();

                data = await _client.GetByteArrayAsync(file.Url);
                string json = System.Text.Encoding.Default.GetString(data);
                Debug.Log("Json for state fetched successfully!");

                //This will probably break in the future. oh well.
                uint version = uint.Parse(json.Substring(json.IndexOf("version") + 9, 1));
                Debug.Log($"Version: {version}");

                //Old state detected
                if(Global.Version > version)
                {
                    Debug.Log("Old state detected!");
                    return (MyState)new MyState().ConvertFromOldState<MyState,MyOldState>(JsonConvert.DeserializeObject<MyOldState>(json));
                } else {
                    Debug.Log("New state detected!");
                    return JsonConvert.DeserializeObject<MyState>(json);
                }
            } catch (System.Exception e)
            {
                //The message was probably deleted
                Debug.Error($"Failed to fetch state. Reason: {e}");
                return null;
            }
        }
    
        public async Task Dispose()
        {
            if(OnDispose != null) await OnDispose.Invoke();

            Debug.Log("Dispose called!");
            //Save state on shutdown
            await SaveState(false);
            Debug.Log("Disposing...");
            MyClient.Dispose();
        }
    }
}