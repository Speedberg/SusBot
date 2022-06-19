namespace Speedberg.Bots.Core
{
    public class State
    {
        /// <summary>
        /// The state version - used to determine between old and new states.
        /// </summary>
        public uint version;
        
        /// <summary>
        /// Unique ID of this instance.
        /// </summary>
        public string uuid;
        /// <summary>
        /// Timestamp of when the first instance of the bot started.
        /// </summary>
        public System.DateTimeOffset firstTimestamp;
        /// <summary>
        /// Timestamp of when this instance of the bot started.
        /// </summary>
        public System.DateTimeOffset instanceStartTime;
        /// <summary>
        /// The ID of this instance - increments for each new instance.
        /// </summary>
        public uint instanceID;

        /// <summary>
        /// Converts an old state to a new state.
        /// </summary>
        /// <typeparam name="T">The newest state.</typeparam>
        /// <typeparam name="T2">The old state.</typeparam>
        /// <param name="oldState">The old state to convert. NOTE: it is best to convert this to whatever your OldState is.</param>
        /// <returns>The converted new state.</returns>
        /// <exception cref="System.NotImplementedException">among us</exception>
        /// smh funny c# generics
        public virtual object ConvertFromOldState<T,T2>(T2 oldState)
        where T : State
        where T2 : State
        {
            throw new System.NotImplementedException("No conversion method has been implemented!");
        }
    }
}