using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Speedberg.Bots.Core.Commands
{
    public static class CommandExecutor
    {

        public static List<CommandHelp> FetchCommandHelp(ClientType clientType, params Command[] commands)
        {
            List<CommandHelp> helpCommands = new List<CommandHelp>();

            if(commands == null || commands.Length <= 0) return helpCommands;
            for(int i = 0; i < commands.Length; i++)
            {
                MethodInfo[] members = (commands[i].GetType()).GetMethods();

                for (int m = 0; m < members.Length; m++)
                {
                    var commandAttribute = (CommandAttribute)Attribute.GetCustomAttribute(members[m],typeof(CommandAttribute));
                    if(commandAttribute == null) continue;
                    if(commandAttribute.ClientType != clientType) continue;
                    
                    var helpAttribute = (HelpAttribute)Attribute.GetCustomAttribute(members[m],typeof(HelpAttribute));
                    if(helpAttribute == null) continue;

                    CommandHelp helpValues = new CommandHelp();
                    helpValues.Name = commandAttribute.Name;
                    helpValues.ClientType = commandAttribute.ClientType;
                    helpValues.Description = helpAttribute.Description;
                    helpValues.Examples = helpAttribute.Example;
                    
                    var parameterAttributes = (ParameterAttribute[])Attribute.GetCustomAttributes(members[m],typeof(ParameterAttribute));
                    if(parameterAttributes != null && parameterAttributes.Length > 0)
                    {
                        CommandHelp.Parameter[] parameters = new CommandHelp.Parameter[parameterAttributes.Length];

                        for(int p = 0; p < parameterAttributes.Length; p++)
                        {
                            parameters[p].Name = parameterAttributes[p].Name;
                            parameters[p].Description = parameterAttributes[p].Help;
                            parameters[p].Optional = parameterAttributes[p].Optional;
                        }

                        helpValues.Parameters = parameters;
                    }

                    helpCommands.Add(helpValues);
                }
            }
            return helpCommands;
        }

        public static Command FetchCommand(string name, params Command[] commands)
        {
            name = name.ToLower();
            for(int i = 0; i < commands.Length; i++)
            {
                MethodInfo[] members = (commands[i].GetType()).GetMethods();

                for (int m = 0; m < members.Length; m++)
                {
                    var attribute = (Commands.CommandAttribute)Attribute.GetCustomAttribute(members[m],typeof(Commands.CommandAttribute));
                    if(attribute == null) continue;

                    if(attribute.Name.ToLower() != name) continue;
                    return commands[i];
                }
            }
            return null;
        }

        public static void Execute<T>(Command command, params object[] parameters) where T : Command
        {
            MethodInfo[] members = typeof(T).GetMethods();
            for (int i = 0; i < members.Length; i++)
            {
                var attribute = (Commands.CommandAttribute)Attribute.GetCustomAttribute(members[i],typeof(Commands.CommandAttribute));
                if (attribute == null) continue;
                ParameterInfo[] parameterInfos = members[i].GetParameters();
                
                if(parameterInfos.Length != parameters.Length) continue;

                bool matches = true;
                for(int p = 0; p < parameterInfos.Length; p++)
                {
                    if(parameterInfos[p].ParameterType != parameters[p].GetType()) matches = false;
                }

                if(!matches) return;

                members[i].Invoke(command,parameters);
                return;
            }
        }

        public static void Execute(Command command, params object[] parameters)
        {
            MethodInfo[] members = (command.GetType()).GetMethods();
            for (int i = 0; i < members.Length; i++)
            {
                var attribute = (Commands.CommandAttribute)Attribute.GetCustomAttribute(members[i],typeof(Commands.CommandAttribute));
                if (attribute == null) continue;
                ParameterInfo[] parameterInfos = members[i].GetParameters();
                
                if(parameterInfos.Length != parameters.Length) continue;

                bool matches = true;
                for(int p = 0; p < parameterInfos.Length; p++)
                {
                    if(parameterInfos[p].ParameterType != parameters[p].GetType()) matches = false;
                }

                if(!matches) return;

                members[i].Invoke(command,parameters);
                return;
            }
        }

        public static async Task ExecuteAsync(Command command, params object[] parameters)
        {
            MethodInfo[] members = (command.GetType()).GetMethods();
            for (int i = 0; i < members.Length; i++)
            {
                var attribute = (Commands.CommandAttribute)Attribute.GetCustomAttribute(members[i],typeof(Commands.CommandAttribute));
                if (attribute == null) continue;
                ParameterInfo[] parameterInfos = members[i].GetParameters();
                
                if(parameterInfos.Length != parameters.Length) continue;

                bool matches = true;
                for(int p = 0; p < parameterInfos.Length; p++)
                {
                    if(parameterInfos[p].ParameterType != parameters[p].GetType()) matches = false;
                }

                if(!matches) return;

                await members[i].InvokeAsync(command,parameters);
                return;
            }
        }

        public static async Task ExecuteSlashAsync(Command command, params object[] parameters)
        {
            MethodInfo[] members = (command.GetType()).GetMethods();
            for (int i = 0; i < members.Length; i++)
            {
                var attribute = (Commands.SlashCommandAttribute)Attribute.GetCustomAttribute(members[i],typeof(Commands.SlashCommandAttribute));
                if (attribute == null) continue;
                ParameterInfo[] parameterInfos = members[i].GetParameters();
                
                if(parameterInfos.Length != parameters.Length) continue;

                bool matches = true;
                for(int p = 0; p < parameterInfos.Length; p++)
                {
                    if(parameterInfos[p].ParameterType != parameters[p].GetType()) matches = false;
                }

                if(!matches) return;

                await members[i].InvokeAsync(command,parameters);
                return;
            }
        }

        public static async Task<Dictionary<ulong,Command>> BuildSlashCommands(DSharpPlus.DiscordClient client, params Command[] commands)
        {
            Dictionary<ulong,Command> slashCommands = new Dictionary<ulong,Command>();

            if(commands == null || commands.Length <= 0) return null;

            for(int i = 0; i < commands.Length; i++)
            {
                MethodInfo[] methods = (commands[i].GetType()).GetMethods();

                for (int m = 0; m < methods.Length; m++)
                {
                    //Gets the SlashCommandAttribute attached to the method
                    var commandAttribute = (SlashCommandAttribute)Attribute.GetCustomAttribute(methods[m],typeof(SlashCommandAttribute));
                    if(commandAttribute == null) continue;

                    DSharpPlus.Entities.DiscordApplicationCommandOption[] options = null;

                    var parameterAttributes = (SlashCommandOption[])Attribute.GetCustomAttributes(methods[m],typeof(SlashCommandOption));
                    if(parameterAttributes != null && parameterAttributes.Length > 0)
                    {
                        options = new DSharpPlus.Entities.DiscordApplicationCommandOption[parameterAttributes.Length];

                        for(int p = 0; p < parameterAttributes.Length; p++)
                        {
                            options[p] = parameterAttributes[p].Option;
                        }
                    }

                    DSharpPlus.Entities.DiscordApplicationCommand command = new DSharpPlus.Entities.DiscordApplicationCommand
                    (
                        commandAttribute.Name,
                        commandAttribute.Description,
                        options,
                        true,
                        DSharpPlus.ApplicationCommandType.SlashCommand
                    );

                    try
                    {
                        DSharpPlus.Entities.DiscordApplicationCommand response = await client.CreateGlobalApplicationCommandAsync(command);
                        slashCommands.Add(response.ApplicationId,commands[i]);
                    } catch(System.Exception e)
                    {
                        //I don't care
                    }
                }
            }
            return slashCommands;
        }

        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }
}