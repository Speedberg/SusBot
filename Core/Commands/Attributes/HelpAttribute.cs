using System;
using System.Collections.Generic;

namespace Speedberg.Bots.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HelpAttribute : Attribute
    {
        private string _description;
        private string[] _examples;

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string[] Example
        {
            get
            {
                return _examples;
            }
        }

        public HelpAttribute(string description, params string[] examples)
        {
            _description = description;
            _examples = examples;
        }
    }
}