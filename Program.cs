using System;
using System.Threading;
using System.Threading.Tasks;
using Speedberg.Bots.Core;

namespace TestBot
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if(args.Length < 1)
            {
                Console.WriteLine("sus sus sus!");
                Environment.Exit(0);
            }

            //Cancels after 5 hours and 30 minutes
            Global.Cts = new CancellationTokenSource(19800000);
            Global.StartTime = System.DateTimeOffset.Now;

            try
            {
                MainAsync(args[0]).GetAwaiter().GetResult();
            } catch(System.Exception e)
            {
                Console.WriteLine("[ERROR]: {0}", e);
            }

            Console.WriteLine("Exiting.");
            System.Environment.Exit(0);
        }

        private static async Task MainAsync(string token)
        {
            DiscordGlobal.Client = new Speedberg.Bots.Core.Discord.Client();
            await DiscordGlobal.Client.Setup(token, "!", new DSharpPlus.Entities.DiscordActivity("Debug Mode"),
            new Speedberg.Bots.Ping(),
            new Speedberg.Bots.SecretAmogus(),
            new Speedberg.Bots.Status(),
            new Speedberg.Bots.Help(),
            new Speedberg.Bots.Markov());

            while(!Global.Cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }
    }
}
