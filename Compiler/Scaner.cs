using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Scaner
    {
        public enum LangPeace
        {
            PlusMinusSign,
            MultDivSign,
            LeftBracket,
            RightBracket,
            Num,
            ID
        }

        public struct ScannedPeaceOfText
        {
            public LangPeace langPeace;
            public int pos, len;
            public ScannedPeaceOfText(LangPeace langPeace, int pos, int len)
            {
                this.langPeace = langPeace;
                this.pos = pos;
                this.len = len;
            }
            public int NextSymbol { get => pos + len; }
        }

        public List<ScannedPeaceOfText> ScanCode(string input)
        {
            List<ScannedPeaceOfText> ret = new List<ScannedPeaceOfText>();

            int state = 1, start = 0;

            for (int i = 0; i < input.Length; i++)
            {
                switch (state)
                {
                    case 1:
                        {
                            if (char.IsWhiteSpace(input[i]))
                                break;
                            if (input[i] == '+' || input[i] == '-')
                                ret.Add(new ScannedPeaceOfText(LangPeace.PlusMinusSign, i, 1));
                            if (input[i] == '*' || input[i] == '/')
                                ret.Add(new ScannedPeaceOfText(LangPeace.MultDivSign, i, 1));
                            if (input[i] == '(')
                                ret.Add(new ScannedPeaceOfText(LangPeace.LeftBracket, i, 1));
                            if (input[i] == ')')
                                ret.Add(new ScannedPeaceOfText(LangPeace.RightBracket, i, 1));
                            if (char.IsDigit(input[i]))
                            {
                                state = 2;
                                start = i;
                            }
                            if (char.IsLetter(input[i]))
                            {
                                state = 3;
                                start = i;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (!char.IsDigit(input[i]))
                            {
                                ret.Add(new ScannedPeaceOfText(LangPeace.Num, start, i - start));
                                state = 1;
                                i--;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (!char.IsDigit(input[i]) && !char.IsLetter(input[i]))
                            {
                                ret.Add(new ScannedPeaceOfText(LangPeace.ID, start, i - start));
                                state = 1;
                                i--;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            switch (state)
            {
                case 2:
                    ret.Add(new ScannedPeaceOfText(LangPeace.Num, start, input.Length - start));
                    break;
                case 3:
                    ret.Add(new ScannedPeaceOfText(LangPeace.ID, start, input.Length - start));
                    break;
                default:
                    break;
            }

            return ret;
        }
    }
}
