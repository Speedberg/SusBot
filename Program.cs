using System;
using System.Threading;
using System.Threading.Tasks;
using Speedberg.Bots.Core;

namespace Speedberg.SusBot
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Global.Version = 1;

            if(args.Length < 1)
            {
                Console.WriteLine("sus sus sus!");
                Environment.Exit(0);
            }

            //Cancels after 5 hours and 54 minutes
            Global.Cts = new CancellationTokenSource(21240000);
            Global.StartTime = System.DateTimeOffset.Now;
            new DiscordGlobal<State, OldState>
            (
                579729416314421268,
                734735424571965451,
                709850883336175669,
                976940974246875207,
                979439267031973948
            );

            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            } catch(System.Exception e)
            {
                Debug.Fatal($"Unexpected crash: {e}");
            }

            //Remove listeners
            DiscordGlobal<State, OldState>.Client.OnStateChanged -= Modules.Egg.OnStateChanged;
            DiscordGlobal<State, OldState>.Client.OnMessageCreateStart -= Modules.Egg.OnMessageCreated;
            DiscordGlobal<State, OldState>.Client.OnReady -= Modules.Egg.OnReady;

            DiscordGlobal<State, OldState>.Client.Dispose().GetAwaiter().GetResult();

            Debug.Log($"Shutodwn successful: runtime - {(System.DateTimeOffset.Now - Global.StartTime).ToString()}");
            Debug.Log("Exiting.");
            System.Environment.Exit(0);
        }

        private static async Task MainAsync(string[] args)
        {
            Global.DebugMode = (args[args.Length - 1] == "DEBUG" || System.OperatingSystem.IsWindows());

            DiscordGlobal<State, OldState>.Client = new Speedberg.Bots.Core.Discord.Client<State,OldState>();

            DiscordGlobal<State, OldState>.Client.OnStateChanged += Modules.Egg.OnStateChanged;
            DiscordGlobal<State, OldState>.Client.OnMessageCreateStart += Modules.Egg.OnMessageCreated;
            DiscordGlobal<State, OldState>.Client.OnReady += Modules.Egg.OnReady;

            DiscordGlobal<State, OldState>.Client.OnMessageCreateStart += async (eventArgs) =>
            {
                if(eventArgs.Channel.Type == DSharpPlus.ChannelType.Private)
                {
                    if(!eventArgs.Author.IsBot)
                    {
                        DSharpPlus.Entities.DiscordEmbedBuilder builder = new DSharpPlus.Entities.DiscordEmbedBuilder();
                            
                        builder.WithTitle("Epic amogus easter egg!!!11!!!1!");
                        string image = "";
                        int randInt = Global.Random.Next(0, 7);
                        
                        if(randInt == 0)
                        {
                            image = "https://c.tenor.com/xvysizpVFvEAAAAM/sus-pizza.gif";
                        } else if(randInt == 1)
                        {
                            image = "https://cdn.discordapp.com/attachments/979467184722022490/987088007481942087/america.webp";
                        } else if(randInt == 2)
                        {
                            image = "https://cdn.discordapp.com/attachments/979467184722022490/987088007712624650/american.png";
                        } else if(randInt == 3)
                        {
                            image = "https://cdn.discordapp.com/attachments/979467184722022490/987088007993622590/gandhi.png";
                        } else if(randInt == 4)
                        {
                            image = "https://cdn.discordapp.com/attachments/979467184722022490/987088008299839548/iphone-sus-pro.jpg";
                        } else if(randInt == 5)
                        {
                            image = "https://cdn.discordapp.com/attachments/979467184722022490/987088348940214282/amogus_maid.png";
                        } else if(randInt == 6)
                        {
                            image = "https://media.discordapp.net/attachments/979467184722022490/987088349422567454/cream.jpg";
                        } else if(randInt == 7)
                        {
                            image = "https://media.discordapp.net/attachments/979467184722022490/987088349720379412/je_sus.jpg";
                        }

                        builder.WithImageUrl(image);
                        builder.WithDescription("[Click here for free Nitro!](https://speedberg.github.io/a/nitro)");

                        await eventArgs.Channel.SendMessageAsync(builder);
                        builder = null;
                    }
                    return;
                }
            };

            await DiscordGlobal<State, OldState>.Client.Setup(args[0], "!", new DSharpPlus.Entities.DiscordActivity("Restarting..."),
                new Speedberg.SusBot.Ping(),
                new Speedberg.SusBot.Help(),
                new Speedberg.SusBot.Info(),
                new Speedberg.SusBot.Shutdown(),

                new Speedberg.SusBot.Modules.Fun.Eject(),
                new Speedberg.SusBot.Modules.Fun.Markov(),

                new Speedberg.SusBot.Modules.Utility.Avatar(),
                new Speedberg.SusBot.Modules.Utility.Google(),
                new Speedberg.SusBot.Modules.Utility.Status(),
                new Speedberg.SusBot.Modules.Utility.Verbose(),
                new Speedberg.SusBot.Modules.Utility.Server()
            );

            while(!Global.Cts.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }
    }
}
