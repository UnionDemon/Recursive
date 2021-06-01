using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler
{
    internal class Identifiers
    {
        private KeyWords keyWords;
        private Regex LoadRegex, IDRegex;
        private List<string> identifiersList;

        public List<string> IdentifiersList { get => identifiersList; }

        public Identifiers()
        {
            keyWords = new KeyWords();
            identifiersList = new List<string>();

            string pattern = @"";
            foreach (string str in keyWords.Words)
            {
                pattern += @"((?<=\b" + str + @"\s+)\w+\b)";
            }
            pattern = pattern.Replace(")(", ")|(");
            LoadRegex = new Regex(pattern);
        }

        public void LoadIdentifiers(string code)
        {
            MatchCollection matchCollection = LoadRegex.Matches(code);
            identifiersList.Clear();
            foreach (Match str in matchCollection)
            {
                identifiersList.Add(str.Value);
            }
            string pattern = @"";
            foreach (string str in identifiersList)
            {
                pattern += @"(\b" + str + @"\b)";
            }
            pattern = pattern.Replace(")(", ")|(");
            IDRegex = new Regex(pattern);
        }

        public MatchCollection FindMatches(string code)
        {
            return IDRegex.Matches(code);
        }
    }
}