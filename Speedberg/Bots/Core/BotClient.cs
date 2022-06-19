using System.Collections.Generic;

namespace Speedberg.Bots.Core
{
    using Commands;

    public class BotClient
    {
        protected string _token;
        protected string _prefix;

        public string Prefix
        {
            get
            {
                return _prefix;
            }
        }

        protected ClientType _clientType;

        public ClientType ClientType
        {
            get
            {
                return _clientType;
            }
        }

        protected Command[] _commands;

        protected List<CommandHelp> _helpCommands;

        public CommandHelp[] HelpCommands
        {
            get
            {
                if(_helpCommands != null) return _helpCommands.ToArray();

                if(_commands != null && _commands.Length > 0)
                {
                    _helpCommands = CommandExecutor.FetchCommandHelp(_clientType,_commands);
                    return _helpCommands.ToArray();
                }

                return null;
            }
        }
    }
}