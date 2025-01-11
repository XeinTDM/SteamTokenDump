using System.Text.RegularExpressions;
using System.Text;

namespace SteamTokenDump
{
    internal static class TokenExtractor
    {
        public static IEnumerable<string> ExtractTokens(byte[] data, int bytesRead, Encoding encoding, Regex regex)
        {
            var matchedTokens = new List<string>();
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < bytesRead; i++)
            {
                char c = (char)data[i];

                if (IsValidCharacter(c))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    if (stringBuilder.Length > 0)
                    {
                        string candidate = stringBuilder.ToString();
                        if (regex.IsMatch(candidate))
                        {
                            matchedTokens.Add(candidate);
                        }
                        stringBuilder.Clear();
                    }
                }
            }

            if (stringBuilder.Length > 0)
            {
                string candidate = stringBuilder.ToString();
                if (regex.IsMatch(candidate))
                {
                    matchedTokens.Add(candidate);
                }
            }

            return matchedTokens;
        }

        private static bool IsValidCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c);
        }
    }
}
