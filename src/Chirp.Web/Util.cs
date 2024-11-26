using System.Text.RegularExpressions;

namespace Chirp.Web;


/// <summary>
/// This method takes in a cheep and returns a list of strings, 
/// which where preceeded by @ meaning they were possible mentions.
/// </summary>
static class Util {
    public static List<string> ExtractMentions(string cheep)
    {
        var mentions = new List<string>();
        var regex = new Regex(@"@(\w+)");
        var matches = regex.Matches(cheep);
        foreach (Match match in matches)
        {
            mentions.Add(match.Groups[1].Value);
        }
        return mentions;
    }
}
