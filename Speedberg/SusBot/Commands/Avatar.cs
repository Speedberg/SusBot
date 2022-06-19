using Speedberg.Bots.Core;
using Speedberg.Bots.Core.Commands;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Speedberg.SusBot.Modules.Utility
{

    public class Avatar : Command
    {
        [Command("avatar",ClientType.Discord)]
        [Help("Displays and fetches the avatar of a user.",
        "avatar",
        "avatar [@mention]",
        "avatar [user ID]")]
        [Parameter("User","The user to fetch the avatar for.",true)]
        [Parameter("User ID","The ID of the user to fetch the avatar for.",true)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            if(message.MentionEveryone)
            {
                await message.RespondAsync("Error: choose a specific user");
                return;
            }

            string[] keywords = GetKeywords(message.Content).ToArray();

            DiscordUser user = null;
            ulong userID = 0;

            if(keywords.Length == 1)
            {
                user = message.Author;
            } else if(message.MentionedUsers.Count > 0)
            {
                user = message.MentionedUsers[0];
            } else if(keywords.Length < 2 || (!ulong.TryParse(keywords[1], out userID)))
            {
                await message.RespondAsync("Error: incorrect parameters idiot");
                return;
            }

            if(userID != 0)
            {
                try
                {
                    user = await DiscordGlobal<State,OldState>.Client.MyClient.GetUserAsync(userID);
                } catch
                {
                    await message.RespondAsync("Error: the user doesn't exist.");
                    return;
                }
            }

            if(user == null)
            {
                await message.RespondAsync("Error: the user doesn't exist.");
                return;
            }

            if(user.Id == DiscordGlobal<State,OldState>.Client.MyClient.CurrentUser.Id)
            {
                await message.RespondAsync("No");
                return;
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.WithTitle("Avatar: " + (user.Username + "#" + user.Discriminator));
            string link = (user.AvatarUrl) ?? user.DefaultAvatarUrl + "?size=1024";
            embed.WithDescription($"[Link]({link})");
            embed.WithImageUrl(link);
            embed.WithFooter(BotGlobal.WebsiteBots);

            await message.RespondAsync(embed.Build());
        }
    }
}