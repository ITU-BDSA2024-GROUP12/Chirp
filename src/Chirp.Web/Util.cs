using System.Text.RegularExpressions;

namespace Chirp.Web;


/// <summary>
/// This method finds all cases in a given string, 
/// which where preceeded by @ making them possible mentions.
/// </summary>
/// <returns>
/// <param name="mentions" > a list of possible mentions </param>
/// </returns>
static class Util {
    public static List<string> ExtractMentions(string cheep)
    {
        var mentions = new List<string>();
        var regex = new Regex(@"@([A-Za-z0-9_\. -]+)");
        var matches = regex.Matches(cheep);
        foreach (Match match in matches)
        {
            mentions.Add(match.Groups[1].Value);
        }
        return mentions;
    }
}
