namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;
    using DSharpPlus;
    using DSharpPlus.Entities;
    using System.Collections.Generic;

    public class Help : Command
    {
        [Command("help",ClientType.Discord)]
        [Help("Provides info about commands.",
        "help",
        "help [page number]",
        "help [command name]")]
        [Parameter("Page Number","The page number to show.",true)]
        [Parameter("Command","A command to find help for.",true)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            CommandHelp[] help = DiscordGlobal.Client.HelpCommands;

            if(help == null || help.Length == 0)
            {
                await message.RespondAsync("Error: no commands found!");
                return;
            }
            
            List<string> keywords = GetKeywords(message.Content);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            if(keywords.Count > 1)
            {
                int pageNumber = 0;
                if(int.TryParse(keywords[1], out pageNumber))
                {
                    
                    embed.WithTitle("Help");
                    embed.WithDescription("Displaying all commands");
                    pageNumber = (pageNumber-1) * 10;
                    if(pageNumber >= help.Length) pageNumber = 0;

                    for(int i = pageNumber; i < pageNumber + 10; i++)
                    {
                        if(i >= help.Length) break;
                        embed.AddField(help[i].Name,help[i].Description);
                    }
                    embed.WithFooter($"Page {(pageNumber/10)+1}/{System.MathF.Ceiling(help.Length / 10f)}");
                } else {
                    for(int i = 0; i < help.Length; i++)
                    {
                        if(help[i].Name == keywords[1])
                        {
                            embed.WithTitle("Help - " + help[i].Name);
                            embed.WithDescription(help[i].Description ?? "No help available.");

                            string examples = "";
                            for(int e = 0; e < help[i].Examples.Length; e++)
                            {
                                examples += "`" + DiscordGlobal.Client.Prefix + help[i].Examples[e] + "`\n";
                            }
                            embed.AddField("Examples",examples);
                            
                            if(help[i].Parameters != null)
                            {
                                for(int p = 0; p < help[i].Parameters.Length; p++)
                                {
                                    embed.AddField("Parameter: " + help[i].Parameters[p].Name + (help[i].Parameters[p].Optional ? " (Optional)" : ""),help[i].Parameters[p].Description);
                                }
                            }
                            break;
                        }
                    }
                }
            } else {
                embed.WithTitle("Help");
                embed.WithDescription("Displaying all commands");

                for(int i = 0; i < 10; i++)
                {
                    if(i >= help.Length) break;
                    embed.AddField(help[i].Name,help[i].Description ?? "N/A");
                }
                embed.WithFooter($"Page 1/{System.MathF.Ceiling(help.Length / 10f)}");
            }

            await message.RespondAsync(embed.Build());
        }

        [Command("help",ClientType.Revolt)]
        [Help("Provides info about commands.","help")]
        public async Task RevoltCommand()
        {
           
        }
    }
}