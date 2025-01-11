namespace SteamTokenDump
{
    internal static class Logger
    {
        public static void Warning(string message)
        {
            WriteColored("[!] ", ConsoleColor.Red, false);
            WriteColored(message, ConsoleColor.White, true);
        }

        public static void Info(string message)
        {
            WriteColored("[?] ", ConsoleColor.Cyan, false);
            WriteColored(message, ConsoleColor.White, true);
        }

        public static void Success(string message)
        {
            WriteColored("[+] ", ConsoleColor.Green, false);
            WriteColored(message, ConsoleColor.White, true);
        }

        public static void Error(string message)
        {
            WriteColored("[x] ", ConsoleColor.Magenta, false);
            WriteColored(message, ConsoleColor.White, true);
        }

        private static void WriteColored(string text, ConsoleColor color, bool newLine)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            Console.ForegroundColor = previousColor;
        }
    }
}
