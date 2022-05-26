using System;
using DSharpPlus.Entities;

namespace Speedberg.Bots.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SlashCommandOption : Attribute
    {
        public string Name;
        public string Description;
        public Type CommandType;
        public bool Required;

        public DiscordApplicationCommandOption Option
        {
            get;
            private set;
        }

        public SlashCommandOption(string name, string description, Type type, bool required)
        {
            Name = name;
            Description = description;
            CommandType = type;
            Option = new DiscordApplicationCommandOption(Name, Description, ConvertType(type), required, autocomplete: true);
        }

        private DSharpPlus.ApplicationCommandOptionType ConvertType(Type type)
        {
            switch(type)
            {
                case Type.SubCommand:
                    return DSharpPlus.ApplicationCommandOptionType.SubCommand;
                case Type.SubCommandGroup:
                    return DSharpPlus.ApplicationCommandOptionType.SubCommandGroup;
                case Type.String:
                    return DSharpPlus.ApplicationCommandOptionType.String;
                case Type.Integer:
                    return DSharpPlus.ApplicationCommandOptionType.Integer;
                case Type.Boolean:
                    return DSharpPlus.ApplicationCommandOptionType.Boolean;
                case Type.User:
                    return DSharpPlus.ApplicationCommandOptionType.User;
                case Type.Channel:
                    return DSharpPlus.ApplicationCommandOptionType.Channel;
                case Type.Role:
                    return DSharpPlus.ApplicationCommandOptionType.Role;
                case Type.Mentionable:
                    return DSharpPlus.ApplicationCommandOptionType.Mentionable;
                case Type.Number:
                    return DSharpPlus.ApplicationCommandOptionType.Number;
                case Type.Attachment:
                    return DSharpPlus.ApplicationCommandOptionType.Attachment;
                default:
                    return DSharpPlus.ApplicationCommandOptionType.String;
            }
        }

        public enum Type
        {
            SubCommand = 1,
            SubCommandGroup,
            String,
            Integer,
            Boolean,
            User,
            Channel,
            Role,
            Mentionable,
            Number,
            Attachment
        }
    }
}