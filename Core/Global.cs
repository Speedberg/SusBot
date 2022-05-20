using DSharpPlus.Entities;

namespace Speedberg.Bots
{
    public static class Global
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
    }
}