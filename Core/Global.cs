using DSharpPlus.Entities;
using System.Threading;

namespace Speedberg.Bots.Core
{
    public enum ClientType
    {
        Discord,
        Revolt
    }

    public static class Global
    {
        public static CancellationTokenSource Cts;
        
        public static System.DateTimeOffset StartTime;
        public static string UptimeDiscord
        {
            get
            {
                return $"<t:{Global.StartTime.ToUnixTimeSeconds().ToString()}:R>";
            }
        }

        public static string Website = "https://speedberg.github.io/";
        public static string WebsiteBots = "https://speedberg.github.io/";
        public static string DiscordBotInvite = "https://speedberg.github.io/a/nitro/";
    }

    public static class DiscordGlobal
    {
        public static readonly ulong OwnerID = 579729416314421268;
        public static readonly ulong BotID = 734735424571965451;

        /// <summary>
        /// Server ID of the server which contains the state channel.
        /// </summary>
        public static readonly ulong StateServerID = 709850883336175669;
        public static readonly ulong StateChannelID = 976940974246875207;
        public static DiscordGuild CachedStateGuild = null;
        public static DiscordChannel CachedStateChannel = null;
        public static DiscordMessage StateMessage = null;

        public static State BotState;

        public static Discord.Client Client;
    }
}