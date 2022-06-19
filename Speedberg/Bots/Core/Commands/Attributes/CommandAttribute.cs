using System;

namespace Speedberg.Bots.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        private string _name;
        private ClientType _clientType;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ClientType ClientType
        {
            get
            {
                return _clientType;
            }
        }

        public CommandAttribute(string name, ClientType clientType)
        {
            _name = name;
            _clientType = clientType;
        }
    }
}