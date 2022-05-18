using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace TestBot
{
    class Program
    {
        private static DiscordClient DiscordClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if(args.Length < 1)
            {
                Console.WriteLine("sus sus sus!");
                Environment.Exit(0);
            }

            MainAsync(args[0]).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string token)
        {
            DiscordClient = new DSharpPlus.DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = DSharpPlus.TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged     
            });

            DiscordClient.MessageCreated += OnMessageCreated;

            await DiscordClient.ConnectAsync(activity: new DiscordActivity("egg",ActivityType.Playing));

            await Task.Delay(-1);
        }

        private static async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if(eventArgs.Message.Content.ToLower().Contains("egg") && !eventArgs.Message.Author.IsBot)
            {
                await eventArgs.Message.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient,":egg:",false));
            }
            if((System.DateTime.Now.ToBinary() % 11) == 0)
            {
                await eventArgs.Channel.SendMessageAsync(DiscordEmoji.FromName(DiscordClient,":egg:"));
            }
        }
    }
}
