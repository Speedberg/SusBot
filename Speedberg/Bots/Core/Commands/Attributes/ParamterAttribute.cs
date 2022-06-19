using System;

namespace Speedberg.Bots.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ParameterAttribute : Attribute
    {
        private string _name;
        private string _help;
        private bool _optional;

        public string Name { get { return _name; }}
        public string Help { get { return _help; }}
        public bool Optional {get {return _optional; }}

        public ParameterAttribute(string name, string help, bool optional = false)
        {
            _name = name;
            _help = help;
            _optional = optional;
        }
    }
}