using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Speedberg.SusBot
{    
    public class Shutdown : Command
    {
        [Command("shutdown",ClientType.Discord)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            if(message.Author.Id != DiscordGlobal<State,OldState>.OwnerID)
            {
                return;
            }
            
            await message.RespondAsync("Attempting shutdown...");
            try
            {
                Global.Cts.Cancel();
            } catch(System.Exception e)
            {
                await message.RespondAsync($"Failed shutdown - reason: {e}");
            }
        }
    }
}