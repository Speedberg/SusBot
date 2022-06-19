namespace Speedberg.Bots.Core.Commands
{
    public struct CommandHelp
    {
        ///<summary>The name of the command</summary>
        public string Name;
        ///<summary>The client this command can be used on</summary>
        public ClientType ClientType;
        ///<summary>The help description of this command</summary>
        public string Description;
        ///<summary>Example commands</summary>
        public string[] Examples;
        ///<summary>The parameters for this command</summary>
        public Parameter[] Parameters;

        public struct Parameter
        {
            ///<summary>The name of the parameter</summary>
            public string Name;
            ///<summary>The help description for this parameter</summary>
            public string Description;
            ///<summary>Boolean if the command is optional or not</summary>
            public bool Optional;
        }
    }
}