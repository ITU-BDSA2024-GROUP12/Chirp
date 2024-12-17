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
        var regex = new Regex(@"@([a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)?)(?=\s|$)");
        var matches = regex.Matches(cheep);
        foreach (Match match in matches)
        {
            mentions.Add(match.Groups[1].Value.Trim());
        }
        return mentions;
    }
}
