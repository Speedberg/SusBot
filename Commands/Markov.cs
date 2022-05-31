namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;
    using DSharpPlus.Entities;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Markov : Command
    {
        //TODO - make this multi-threaded - give option to cache data to file on server? (NO - GDPR :(((((((()))))))))
        private static Dictionary<string, List<string>> bigrams = new Dictionary<string,List<string>>();

        [Command("markov",ClientType.Discord)]
        [Help("Generates a markov chain from a given length and starting word.","markov [word] [length]")]
        [Parameter("Start Word","The word to start with.",false)]
        [Parameter("Chain Length","The length of the chain",false)]
        public async Task DiscordCommand(DiscordMessage message, DiscordChannel channel)
        {
            try
            {
                List<string> keywords = GetKeywords(message.Content);
                if(keywords.Count < 3) throw new System.Exception("Missing parameters idiot");
                
                int chainLength = 0;

                if(!int.TryParse(keywords[2],out chainLength))
                {
                    throw new System.Exception("Type in a number idiot");
                }

                System.Random random = new System.Random();
                string currentWord = keywords[1];
                string result = keywords[1];

                if(chainLength > 50) chainLength = 50;

                if (bigrams.Count < 1)
                {
                    var messages = await channel.GetMessagesAsync(10000);

                    foreach(var msg in messages)
                    {
                        if(message.Author.IsBot) continue;
                        var sussyWords = msg.Content.Split(' ');
                        var filteredWords = new List<string>();
                        foreach(var sussyWord in sussyWords)
                        {
                            var filtered = sussyWord.ToLower();
                            filtered = Regex.Replace(filtered, @"\W+", "");  
                            filtered = Regex.Replace(filtered, @"^\d+", "");
                            if(filtered == "" || filtered == " ") continue;
                            filteredWords.Add(filtered);
                        }

                        if(filteredWords.Count < 2) continue;
                        for(int i = 0; i < filteredWords.Count-1; i++)
                        {
                            if(!bigrams.ContainsKey(filteredWords[i]))
                            {
                                bigrams.Add(filteredWords[i],new List<string>());
                            }
                            bigrams[filteredWords[i]].Add(filteredWords[i+1]);
                        }
                    }
                }

                for(int i = 0; i < chainLength; i++)
                {
                    if(!bigrams.ContainsKey(currentWord))
                    {
                        break;
                    }
                    var next = bigrams[currentWord][random.Next(0,bigrams[currentWord].Count)];
                    result += " " + next;
                    currentWord = next;
                }
                await message.RespondAsync(result);

            } catch(System.Exception e)
            {
                await message.RespondAsync($"Error: {e.Message}");
            }
        }
    }
}