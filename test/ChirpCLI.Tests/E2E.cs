using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class ChirpEndToEndTestsUsingProcessStartInfo
{
    //private readonly string _chirpExecutablePath = "../../../../../src/Chirp.CLI/bin/Debug/net7.0/Chirp.CLI.exe";
    private readonly string _chirpDllPath = Path.Combine(AppContext.BaseDirectory, "Chirp.CLI.dll");

    [Fact]
    public async Task TestCheepCommand()
    {
        var cheepMessage = "e2e test successful!";
        await RunChirpCommand($"cheep \"{cheepMessage}\"");

        // Success is verified by not receiving an error message.
    }

    [Fact]
    public async Task TestReadCommand()
    {
        var output = await RunChirpCommand("read");

        
        var expectedCheepMessage = "hello";
        Assert.Contains(expectedCheepMessage, output);
    }


    private async Task<string> RunChirpCommand(string arguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet", 
            Arguments = $"{_chirpDllPath} {arguments}", 
            RedirectStandardOutput = true,   
            RedirectStandardError = true,    
            UseShellExecute = false,         
            CreateNoWindow = true            
        };


        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start(); 

            
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit(); 

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Error running Chirp command: {error}");
            }

            return output;
        }
    }
}
