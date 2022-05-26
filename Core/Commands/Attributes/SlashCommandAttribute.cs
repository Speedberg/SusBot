using System;
using DSharpPlus.Entities;

namespace Speedberg.Bots.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SlashCommandAttribute : Attribute
    {
        private string _name;
        private string _description;

        public string Name => _name;
        public string Description => _description;

        public SlashCommandAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }
    }
}