namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;
    using DSharpPlus.Entities;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class Markov : Command
    {
        private static Dictionary<ulong, MarkovGuild> Database = new Dictionary<ulong, MarkovGuild>();
        private static System.Random Random = new System.Random();

        [Command("markov",ClientType.Discord)]
        [Help("Generates a markov chain from a given length and starting word.","markov [word] [length]")]
        [Parameter("Start Word","The word to start with.",false)]
        [Parameter("Chain Length","The length of the chain",false)]
        public async Task DiscordCommand(DiscordMessage message, DiscordChannel channel)
        {
            try
            {
                List<string> keywords = GetKeywords(message.Content);
                if(keywords.Count > 3 || keywords.Count < 2) throw new System.Exception("Missing parameters idiot");
                
                int chainLength = 0;

                if(keywords.Count == 3 && !int.TryParse(keywords[2],out chainLength))
                {
                    throw new System.Exception("Type in a number idiot");
                }

                string currentWord = keywords[1];

                if(chainLength > 100 || chainLength <= 0) chainLength = 100;

                if(!Database.ContainsKey((ulong)channel.GuildId) || !Database[(ulong)channel.GuildId].Channels.ContainsKey(channel.Id))
                {
                    await message.RespondAsync("Creating cache! This usually takes up to 5 minutes");
                    await Task.Run(() => MarkovThread(message,channel,currentWord,chainLength));
                    return;
                } else {
                    string result = GenerateSentence((ulong)channel.GuildId, channel.Id, currentWord, chainLength);
                    await message.RespondAsync(result);
                }

            } catch(System.Exception e)
            {
                await message.RespondAsync($"Error: {e.Message}");
            }
        }

        private async void MarkovThread(DiscordMessage message, DiscordChannel channel, string startWord, int chainLength)
        {
            try
            {
                System.Console.WriteLine($"Generating bigrams for guild {channel.Guild.Name} in channel {channel.Name}.");
                await ConstructBigrams(channel);
                System.Console.WriteLine($"Bigrams successfully made for guild {channel.Guild.Name} in channel {channel.Name}!");
                string result = GenerateSentence((ulong)channel.GuildId, channel.Id, startWord, chainLength);
                await message.RespondAsync(result);
            }catch (System.Exception e)
            {
                System.Console.WriteLine($"Bigrams failed for guild {channel.Guild.Name} in channel {channel.Name} - reason: {e}");
                await message.RespondAsync($"Error: ohno");
            }
        }

        private async Task ConstructBigrams(DiscordChannel channel)
        {
            if(!Database.ContainsKey((ulong)channel.GuildId))
            {
                Database.Add((ulong)channel.GuildId, new MarkovGuild((ulong)channel.GuildId));
            } else if(Database[(ulong)channel.GuildId].Channels.ContainsKey(channel.Id))
            {
                return;
            }

            var messages = await channel.GetMessagesAsync(10000);

            MarkovBigrams bigrams = new MarkovBigrams();

            foreach(var msg in messages)
            {
                if(msg.Author.IsBot) continue;
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
                    if(!bigrams.Bigrams.ContainsKey(filteredWords[i]))
                    {
                        bigrams.Bigrams.Add(filteredWords[i],new List<string>());
                    }
                    bigrams.Bigrams[filteredWords[i]].Add(filteredWords[i+1]);
                }
            }
            Database[(ulong)channel.GuildId].Channels.Add(channel.Id, bigrams);
        }

        private string GenerateSentence(ulong guildID, ulong channelID, string startWord, int chainLength)
        {
            string currentWord = startWord;
            string result = currentWord;

            MarkovBigrams bigrams = Database[guildID].Channels[channelID];
            for(int i = 0; i < chainLength; i++)
            {
                if(!bigrams.Bigrams.ContainsKey(currentWord))
                {
                    break;
                }
                var next = bigrams.Bigrams[currentWord][Random.Next(0,bigrams.Bigrams[currentWord].Count)];
                result += " " + next;
                currentWord = next;
            }
            return result;
        }
    
        private struct MarkovGuild
        {
            public ulong ID;
            public Dictionary<ulong, MarkovBigrams> Channels;

            public MarkovGuild(ulong id)
            {
                ID = id;
                Channels = new Dictionary<ulong, MarkovBigrams>();
            }
        }

        private class MarkovBigrams
        {
            public Dictionary<string, List<string>> Bigrams = new Dictionary<string, List<string>>();
        }
    }
}