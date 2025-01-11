using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;

namespace SteamTokenDump
{
    internal sealed partial class Program
    {
        private static readonly Regex TokenRegex = TokenRegexGen();

        public static async Task Main()
        {
            Logger.Info("Scanning started...");

            var steamProcesses = Process.GetProcessesByName("steam");

            if (steamProcesses.Length == 0)
            {
                Logger.Warning("No Steam process found!");
                return;
            }

            var tokens = await MemoryScanner.ScanProcessMemoryAsync(steamProcesses[0], Encoding.ASCII, TokenRegex);

            if (tokens.Count > 0)
            {
                foreach (var token in tokens)
                {
                    Logger.Success($"Refresh token: {token}");
                }
            }
            else
            {
                Logger.Info("No tokens found.");
            }

            Logger.Info("Scanning finished... (Press Enter to exit)");
            Console.ReadLine();
        }

        [GeneratedRegex(@"eyAidHlwIjogIkpXVCIsICJhbGciOiAiRWREU0EiIH0[0-9a-zA-Z\.\-_]+", RegexOptions.Compiled)]
        private static partial Regex TokenRegexGen();
    }
}
