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
        public static uint Version;
        public static CancellationTokenSource Cts;

        public static bool DebugMode = false;

        public static System.DateTimeOffset StartTime;
        public static System.Random Random = new System.Random();
    }

    public class DiscordGlobal<T,T2>
    where T : State, new()
    where T2 : State, new()
    {
        public static Discord.Client<T,T2> Client;
        public static CancellationTokenSource StateDetectionCts;

        public static ulong OwnerID { get; private set; }
        public static ulong BotID { get; private set; }

        /// <summary>
        /// Server ID of the server which contains the state channel.
        /// </summary>
        public static ulong StateServerID { get; private set; }
        public static ulong StartupChannelID { get; private set; }
        public static ulong ShutdownChannelID { get; private set; }
        public static DiscordGuild CachedStateGuild = null;
        public static DiscordChannel CachedStartupChannel = null;
        public static DiscordChannel CachedShutdownChannel = null;
        public static ulong LastStartupMessageID = 0;

        public static string TotalUptimeToDiscordTimestamp
        {
            get
            {
                return $"<t:{Client.BotState.firstTimestamp.ToUnixTimeSeconds().ToString()}:R>";
            }
        }

        public static string InstanceUptimeToDiscordTimestamp
        {
            get
            {
                return $"<t:{Global.StartTime.ToUnixTimeSeconds().ToString()}:R>";
            }
        }

        public DiscordGlobal(ulong OwnerID, ulong BotID, ulong StateServerID, ulong StartupChannelID, ulong ShutdownChannelID)
        {
            DiscordGlobal<T,T2>.OwnerID = OwnerID;
            DiscordGlobal<T,T2>.BotID = BotID;
            DiscordGlobal<T,T2>.StateServerID = StateServerID;
            DiscordGlobal<T,T2>.StartupChannelID = StartupChannelID;
            DiscordGlobal<T,T2>.ShutdownChannelID = ShutdownChannelID;
        }
    }
}