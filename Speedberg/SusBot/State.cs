namespace Speedberg.SusBot
{
    public class State : Speedberg.Bots.Core.State
    {
        public ulong eggCount;

        public State()
        {
            version = 1;
            eggCount = 0;
        }

        public override object ConvertFromOldState<T, T2>(T2 oldState)
        {
            return new State();
        }
    }    
}