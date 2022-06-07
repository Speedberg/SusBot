namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json;

    using DSharpPlus.Entities;

    public class Status : Command
    {

        [Command("status",ClientType.Discord)]
        [Help("Provides info about the statuses of various apps.",
        "status",
        "status [app name]")]
        [Parameter("App Name","The name of the app:\n`discord`\n`smhw`\n`spotify`\n`github`\n`twitter`\n`reddit`",true)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            string[] keywords = GetKeywords(message.Content).ToArray();

            if(keywords.Length > 2) return;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTitle("Status");
            embed.WithFooter(Global.WebsiteBots);

            if(keywords.Length == 1)
            {
                embed.WithDescription("Displaying the status for this bot.");
                embed.AddField("Total time this bot has been in service",Global.UptimeGlobalDiscord,true);
                embed.AddField("Instance Uptime",Global.UptimeDiscord,true);
                embed.AddField("Ping", DiscordGlobal.Client.DiscordClient.Ping.ToString(), true);
                embed.AddField("Instance", DiscordGlobal.BotState.instanceID.ToString());
            }
            else
            {
                switch(keywords[1])
                {
                    case "reddit":
                        var responseReddit = GetAtlassianStatus<RedditStatusJSON.Root>("https://www.redditstatus.com/api/v2/status.json");
                        embed.WithDescription("Displaying the status for Reddit");
                        embed.WithUrl(responseReddit.Page.Url);
                        embed.AddField("Status Indicator",responseReddit.Status.Indicator,true);
                        embed.AddField("Status Description",responseReddit.Status.Description,true);
                        embed.AddField("Last Update",responseReddit.Page.UpdatedAt.ToString(),true);
                        embed.WithColor(new DiscordColor("#FF5700"));
                        break;
                    case "twitter":
                        var responseTwitter = GetAtlassianStatus<TwitterStatusJSON.Root>("https://api.twitterstat.us/api/v2/status.json");
                        embed.WithDescription("Displaying the status for Twitter");
                        embed.WithUrl(responseTwitter.Page.Url);
                        embed.AddField("Status Indicator",responseTwitter.Status.Indicator,true);
                        embed.AddField("Status Description",responseTwitter.Status.Description,true);
                        embed.AddField("Last Update",responseTwitter.Page.UpdatedAt.ToString(),true);
                        embed.WithColor(new DiscordColor("#1DA1F2"));
                        break;
                    case "github":
                        var responseGithub = GetAtlassianStatus<GithubStatusJSON.Root>("https://discordstatus.com/api/v2/status.json");
                        embed.WithDescription("Displaying the status for Github");
                        embed.WithUrl(responseGithub.page.url);
                        embed.AddField("Status Indicator",responseGithub.status.indicator,true);
                        embed.AddField("Status Description",responseGithub.status.description,true);
                        embed.AddField("Last Update",responseGithub.page.updated_at.ToShortDateString(),true);
                        embed.WithColor(new DiscordColor("#000000"));
                        break;
                    case "discord":
                        var responseDiscord = GetAtlassianStatus<DiscordStatusJSON.Root>("https://discordstatus.com/api/v2/status.json");
                        embed.WithDescription("Displaying the status for Discord");
                        embed.WithUrl(responseDiscord.page.url);
                        embed.AddField("Status Indicator",responseDiscord.status.indicator,true);
                        embed.AddField("Status Description",responseDiscord.status.description,true);
                        embed.AddField("Last Update",responseDiscord.page.updated_at.ToShortDateString(),true);
                        embed.WithColor(new DiscordColor("#5865F2"));
                        break;
                    case "smhw":
                        var responseSmhw = GetAtlassianStatus<SmhwStatusJSON.Root>("https://status.satchelone.com/api/v2/summary.json");
                        embed.WithDescription("Displaying the status for Show My Homework");
                        embed.WithUrl(responseSmhw.page.url);
                        embed.AddField("Status Indicator",responseSmhw.status.indicator,true);
                        embed.AddField("Status Description",responseSmhw.status.description,true);
                        embed.AddField("Last Update",responseSmhw.page.updated_at.ToShortDateString(),true);
                        embed.WithColor(new DiscordColor(0.0f,1.0f,0.0f));
                        break;
                    case "spotify":
                        var responseSpotify = GetAtlassianStatus<SpotifyStatusJSON.Root>("https://status.spotify.dev/api/v2/summary.json");
                        embed.WithDescription("Displaying the status for Spotify");
                        embed.WithUrl(responseSpotify.page.url);
                        embed.AddField("Status Indicator",responseSpotify.status.indicator,true);
                        embed.AddField("Status Description",responseSpotify.status.description,true);
                        embed.AddField("Last Update",responseSpotify.page.updated_at.ToShortDateString(),true);
                        embed.WithColor(new DiscordColor("#1DB954"));
                        break;
                }
            }

            await message.RespondAsync(embed.Build());
        }

        private T GetAtlassianStatus<T>(string url) where T : AtlassianStatusResponse.Root
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            T json;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                json = JsonConvert.DeserializeObject<T>(result); 
            }

            return json;
        }
    }
}