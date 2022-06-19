using System.Linq;
using System.Collections.Generic;

namespace Speedberg.Bots.Core
{
    public class Command
    {
        //TODO - List -> Array
        ///<summary>Separates a message into keywords - includes the name of the command.</summary>
        public List<string> GetKeywords(string message)
        {
            //TODO - change the '1' to match prefix length
           return message.Substring(1).Split(' ').ToList().ConvertAll(s => s.ToLower());
        }
    }
}