namespace Speedberg.Bots
{
    using Core;
    using Core.Commands;
    using System.Threading.Tasks;

    using DSharpPlus.Entities;

    public class Eject : Command
    {
        private static int impostors = 2;
        private static int players = 10;
        private static int baseHash = 0;
        private static System.Random Random;

        [Command("eject",ClientType.Discord)]
        public async Task DiscordCommand(DiscordMessage message)
        {
            if(GetKeywords(message.Content).Count < 2)
            {
                await message.RespondAsync("Error: idiot");
                return;
            }

            string sussy = GetKeywords(message.Content)[1];
            string output = "";
            if(baseHash == 0) baseHash = System.DateTime.Now.GetHashCode();

            Random = new System.Random(sussy.GetHashCode() + baseHash);

            double rand = Random.NextDouble();
            if(rand < 0.5d)
            {
                impostors -= 1;
                output += $"{sussy} was an Impostor.";
            } else {
                players -= 1;
                output += $"{sussy} was not an Impostor.";
            }

            if(players <= 0)
            {
                output += "\nImpostors win\nDEFEAT";
                impostors = 2;
                players = 10;
                baseHash = 0;
            } else if(impostors <= 0)
            {
                output += "\nCrewmates win\nVICTORY";
                impostors = 2;
                players = 10;
                baseHash = 0;
            } else {
                output += $"\n{impostors} Impostors remain";
            }
            await message.RespondAsync(output);
        }
    }
}