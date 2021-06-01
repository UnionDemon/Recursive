using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler
{
    internal class KeyWords
    {
        private List<string> words;
        private Regex regex;

        public KeyWords()
        {
            words = new List<string>();
            words.Add("int");
            words.Add("double");
            words.Add("class");
            words.Add("string");
            words.Add("struct");
            words.Add("float");

            string pattern = @"";
            foreach (string str in words)
            {
                pattern += @"(\b" + str + @"\b)";
            }
            pattern = pattern.Replace(")(", ")|(");
            regex = new Regex(pattern);
        }

        public List<string> Words { get => words; }

        public MatchCollection FindMatches(string code)
        {
            return regex.Matches(code);
        }
    }
}