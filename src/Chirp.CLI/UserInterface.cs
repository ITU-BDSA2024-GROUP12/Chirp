namespace Chirp.CLI
{
    
    /*
     * Class for handling commandline output
     */
    public static class UserInterface
    {
        //takes and prints collection of cheep object
        public static void PrintCheeps(IEnumerable<Cheep> cheeps){ 
            foreach (var cheep in cheeps)
            {
                var message = cheep.Message;
                var author = cheep.Author;
                DateTime timestamp = UnixTimeStampToDateTime(cheep.Timestamp);
                Console.WriteLine(author + " @ " + timestamp + ": " + message);
            }
        }
        /*
         * method for converting the ToUnixTimeSeconds (seconds since 1970-01-01) to a DateTime
         * Taken from stackoverflow https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
         */
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp )
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }
    }
  
}