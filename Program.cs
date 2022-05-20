using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using Speedberg.Bots;

using System.Net.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TestBot
{
    class Program
    {
        private static DiscordClient DiscordClient;
        private static CancellationTokenSource Cts;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if(args.Length < 1)
            {
                Console.WriteLine("sus sus sus!");
                Environment.Exit(0);
            }

            //Cancels after 5 hours and 30 minutes
            Cts = new CancellationTokenSource(19800000);

            try
            {
                MainAsync(args[0]).GetAwaiter().GetResult();
            } catch(System.Exception e)
            {
                Console.WriteLine("[ERROR]: {0}", e);
            }

            //Save state on shutdown
            SaveState("Shutdown").GetAwaiter().GetResult();
            Console.WriteLine("Disconnecting and disposing...");
            DiscordClient.Dispose();
            System.Environment.Exit(0);
        }

        private static async Task MainAsync(string token)
        {
            DiscordClient = new DSharpPlus.DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = DSharpPlus.TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged     
            });

            DiscordClient.MessageCreated += OnMessageCreated;

            await DiscordClient.ConnectAsync(activity: new DiscordActivity("for bugs",ActivityType.Watching));

            DiscordClient.Ready += async (client, args) =>
            {
                Global.CachedStateGuild = await DiscordClient.GetGuildAsync(Global.StateServerID);
                if(Global.CachedStateGuild == null)
                {
                    Console.WriteLine("[ERROR] Could not find State Guild.");
                    return;
                }

                Global.CachedStateChannel = await DiscordClient.GetChannelAsync(Global.StateChannelID);
                if(Global.CachedStateChannel == null || Global.CachedStateChannel.GuildId != Global.StateServerID)
                {
                    Console.WriteLine("[ERROR] Could not find State Channel.");
                    return;
                }

                //Download current state
                State currentState = await FetchState(Global.CachedStateChannel.LastMessageId);

                if(currentState == null)
                {
                    Console.WriteLine("Creating new state...");
                    currentState = new State();
                    currentState.startedAt = DateTime.Now;
                    currentState.instanceID = -1;
                }

                //Update state values
                currentState.instanceID += 1;
                currentState.uuid = Guid.NewGuid().ToString();
                Global.BotState = currentState;
                Console.WriteLine("Bot UUID: {0} Instance: {1}", currentState.uuid,currentState.instanceID);

                //Upload new state
                await SaveState("Startup");
            };

            while(!Cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }

        private static async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if(eventArgs.Author.IsBot && eventArgs.Author.Id == Global.BotID)
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
        }

        private static async Task DetectStateChange(DiscordChannel channel, DiscordMessage message)
        {
            if(channel.GuildId != Global.StateServerID) return;
            if(channel.Id != Global.StateChannelID) return;

            State fileState = await FetchState(message.Id);

            if(fileState == null) return;
            if(fileState.uuid == Global.BotState.uuid) return;

            //New instance detected - dispose this client gracefully
            if(fileState.instanceID > Global.BotState.instanceID)
            {
                Console.WriteLine("death");
                Cts.Cancel();
            } else
            {
                //Old state values - copy these
                fileState.uuid = Global.BotState.uuid;
                fileState.instanceID = Global.BotState.instanceID;

                Global.BotState = fileState;
                Console.WriteLine("Loaded old state!");
            }
        }

        private static async Task SaveState(string type)
        {
            try
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();

                string json = JsonConvert.SerializeObject(Global.BotState);
                byte[] byteArray = Encoding.ASCII.GetBytes(json);

                using(MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    builder.WithContent(type);
                    builder.WithFile("State.json", memoryStream);
                    Console.WriteLine("Saving state!");
                    await DiscordClient.SendMessageAsync(Global.CachedStateChannel, builder);
                }
            } catch(Exception e)
            {
                Console.WriteLine("[ERROR]: {0}", e);
            }
        }

        private static async Task<State> FetchState(ulong? messageID)
        {
            byte[] data = null;

            if (messageID == null) return null;

            try
            {
                DiscordMessage lastState = await Global.CachedStateChannel.GetMessageAsync((ulong)messageID);
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
    }
}
