using Speedberg.Bots.Core;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace Speedberg.SusBot.Modules
{
    public static class Egg
    {
        private static DiscordEmoji egg = null;

        public async static Task OnStateChanged()
        {
            if (Egg.egg == null)
            {
                Egg.egg = DiscordEmoji.FromName(DiscordGlobal<State, OldState>.Client.MyClient, ":egg:", false);
            }

            Debug.Log($"Eggs detected from old state: {((SusBot.State)DiscordGlobal<State, OldState>.Client.BotState).eggCount}");
            try
            {
                await DiscordGlobal<State, OldState>.Client.MyClient.UpdateStatusAsync
                (
                    new DiscordActivity
                    (
                    $"{((SusBot.State)DiscordGlobal<State, OldState>.Client.BotState).eggCount} {egg}"
                    )
                );
            }
            catch (System.Exception b) { }
        }

        public async static Task OnMessageCreated(MessageCreateEventArgs eventArgs)
        {
            if(eventArgs.Message.Content.ToLower().Contains("egg") && !eventArgs.Author.IsBot)
            {
                if(egg == null) egg = DiscordEmoji.FromName(DiscordGlobal<State, OldState>.Client.MyClient, ":egg:", false);

                await eventArgs.Message.CreateReactionAsync(egg);

                ((SusBot.State)DiscordGlobal<State,OldState>.Client.BotState).eggCount += 1;
                try
                {
                    await DiscordGlobal<State,OldState>.Client.MyClient.UpdateStatusAsync
                    (
                        new DiscordActivity
                        (
                        $"{((SusBot.State)DiscordGlobal<State,OldState>.Client.BotState).eggCount} {egg}"
                        )
                    );
                } catch(System.Exception impostor)
                {
                    //eject
                }
                if((System.DateTime.Now.ToBinary() % 77) == 0)
                {
                    await eventArgs.Channel.SendMessageAsync(egg);
                }
            }
        }

        public static async Task OnReady(ReadyEventArgs eventArgs)
        {
            await DiscordGlobal<State, OldState>.Client.MyClient.UpdateStatusAsync
            (
                new DiscordActivity
                (
                    $"{((SusBot.State)DiscordGlobal<State,OldState>.Client.BotState).eggCount} {egg}"
                )
            );
        }
    }
}