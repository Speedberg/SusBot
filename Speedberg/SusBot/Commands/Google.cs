using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Speedberg.SusBot.Modules.Utility
{
    public class Google : Command
    {
        [Command("google",ClientType.Discord)]
        [Help("Let me google that for you.","google [query]")]
        [Parameter("Query","The thing to search",false)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            string query = message.Content.Substring(DiscordGlobal<State,OldState>.Client.Prefix.Length + 7);
            query = query.Replace(" ", "%20");
            await message.RespondAsync("https://lmgtfy.app/#gsc.tab=0&gsc.q=" + query);
        }
    }
}