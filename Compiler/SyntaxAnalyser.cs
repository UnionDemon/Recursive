using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class SyntaxAnalyser
    {
        public struct ErrorInText
        {
            public ErrorInText(int position, int length, string comment = "Неожиданный символ")
            {
                this.position = position;
                this.length = length;
                this.comment = comment;
            }
            public int position, length;
            public string comment;
        }
        public struct CheckResult
        {
            public List<ErrorInText> errors;
            public List<string> analyseSequence;
            public bool correct;
            public int startPos, nextStartPos;
            public CheckResult(bool correct)
            {
                errors = new List<ErrorInText>();
                analyseSequence = new List<string>();
                this.correct = correct;
                startPos = nextStartPos = 0;
            }
        }

        int openBracketsCount;

        public CheckResult Analyse(List<Scaner.ScannedPeaceOfText> scaned)
        {
            CheckResult ret = new CheckResult();

            openBracketsCount = 0;

            ret = E(scaned, 0);

            return ret;
        }

        private CheckResult E(List<Scaner.ScannedPeaceOfText> scaned, int currentPos)
        {
            CheckResult ret = new CheckResult(true);
            ret.analyseSequence.Add("E");
            ret.startPos = currentPos;
            if (currentPos >= scaned.Count)
            {
                ret.correct = false;
                ret.nextStartPos = currentPos;
                return ret;
            }

            CheckResult tRes = T(scaned, currentPos);
            CheckResult aRes = A(scaned, tRes.nextStartPos);

            if (tRes.correct)
            {
                ret.analyseSequence.AddRange(tRes.analyseSequence);
                ret.errors.AddRange(tRes.errors);
            }
            if (aRes.correct)
            {
                ret.analyseSequence.AddRange(aRes.analyseSequence);
                ret.errors.AddRange(aRes.errors);
            }
            ret.correct = tRes.correct && aRes.correct;
            ret.nextStartPos = aRes.nextStartPos;
            return ret;
        }
        private CheckResult A(List<Scaner.ScannedPeaceOfText> scaned, int currentPos)
        {
            CheckResult ret = new CheckResult(true);
            ret.startPos = currentPos;
            if (currentPos >= scaned.Count)
            {
                ret.analyseSequence.Add("A(e)");
                ret.nextStartPos = currentPos;
                return ret;
            }
            CheckResult tRes, aRes;

            //int a = 1 + 1) + 4;

            switch (scaned[currentPos].langPeace)
            {
                case Scaner.LangPeace.PlusMinusSign:
                    ret.analyseSequence.Add("A(+/-TA)");
                    currentPos++;
                    tRes = T(scaned, currentPos);
                    aRes = A(scaned, tRes.nextStartPos);

                    if (tRes.correct)
                    {
                        ret.analyseSequence.AddRange(tRes.analyseSequence);
                        ret.errors.AddRange(tRes.errors);
                        if (aRes.correct)
                        {
                            ret.analyseSequence.AddRange(aRes.analyseSequence);
                            ret.errors.AddRange(aRes.errors);
                        }
                    }
                    if (!tRes.correct)
                        ret.errors.Add(new ErrorInText(scaned[currentPos - 1].NextSymbol, 1, "Требуется терм(+/-)"));
                    ret.nextStartPos = aRes.nextStartPos;
                    return ret;
                case Scaner.LangPeace.LeftBracket:
                case Scaner.LangPeace.Num:
                case Scaner.LangPeace.ID:
                    if (currentPos == 0)
                        break;
                    ret.analyseSequence.Add("A(+/-TA)");
                    ret.errors.Add(new ErrorInText(scaned[currentPos - 1].NextSymbol, 1, "Требуется оператор"));

                    tRes = T(scaned, currentPos);
                    aRes = A(scaned, tRes.nextStartPos);

                    ret.analyseSequence.AddRange(tRes.analyseSequence);
                    ret.errors.AddRange(tRes.errors);
                    if (aRes.correct)
                    {
                        ret.analyseSequence.AddRange(aRes.analyseSequence);
                        ret.errors.AddRange(aRes.errors);
                    }
                    ret.nextStartPos = aRes.nextStartPos;
                    return ret;
                case Scaner.LangPeace.RightBracket:
                    if (openBracketsCount != 0)
                        break;
                    ret.analyseSequence.Add("A(+/-TA)");
                    ret.errors.Add(new ErrorInText(scaned[currentPos].pos, 1, "Требуется \"(\""));
                    ret.nextStartPos = currentPos + 1;
                    break;
                default:
                    break;
            }

            ret.analyseSequence.Add("A(e)");
            ret.nextStartPos = currentPos;
            return ret;
        }
        private CheckResult T(List<Scaner.ScannedPeaceOfText> scaned, int currentPos)
        {
            CheckResult ret = new CheckResult(true);
            ret.analyseSequence.Add("T");
            ret.startPos = currentPos;
            if (currentPos >= scaned.Count)
            {
                ret.correct = false;
                ret.nextStartPos = currentPos;
                return ret;
            }

            CheckResult oRes = O(scaned, currentPos);
            CheckResult bRes = B(scaned, oRes.nextStartPos);

            if (oRes.correct)
            {
                ret.analyseSequence.AddRange(oRes.analyseSequence);
                ret.errors.AddRange(oRes.errors);
            }
            if (bRes.correct)
            {
                ret.analyseSequence.AddRange(bRes.analyseSequence);
                ret.errors.AddRange(bRes.errors);
            }
            ret.correct = oRes.correct && bRes.correct;
            ret.nextStartPos = bRes.nextStartPos;
            return ret;
        }
        private CheckResult B(List<Scaner.ScannedPeaceOfText> scaned, int currentPos)
        {
            CheckResult ret = new CheckResult(true);
            ret.startPos = currentPos;
            if (currentPos >= scaned.Count)
            {
                ret.analyseSequence.Add("B(e)");
                ret.nextStartPos = currentPos;
                return ret;
            }

            if (scaned[currentPos].langPeace == Scaner.LangPeace.MultDivSign)
            {
                ret.analyseSequence.Add("B(*//OB)");
                currentPos++;
                CheckResult oRes = O(scaned, currentPos);
                CheckResult bRes = B(scaned, oRes.nextStartPos);


                if (oRes.correct)
                {
                    ret.analyseSequence.AddRange(oRes.analyseSequence);
                    ret.errors.AddRange(oRes.errors);
                    if (bRes.correct)
                    {
                        ret.analyseSequence.AddRange(bRes.analyseSequence);
                        ret.errors.AddRange(bRes.errors);
                    }
                }
                if (!oRes.correct)
                    ret.errors.Add(new ErrorInText(scaned[currentPos - 1].NextSymbol, 1, "Требуется терм(*//)"));
                ret.nextStartPos = bRes.nextStartPos;
                return ret;
            }

            ret.analyseSequence.Add("B(e)");
            ret.nextStartPos = currentPos;
            return ret;
        }
        private CheckResult O(List<Scaner.ScannedPeaceOfText> scaned, int currentPos)
        {
            CheckResult ret = new CheckResult(true);
            ret.startPos = currentPos;
            if (currentPos >= scaned.Count)
            {
                ret.analyseSequence.Add("O");
                ret.correct = false;
                ret.nextStartPos = currentPos;
                return ret;
            }

            if (scaned[currentPos].langPeace == Scaner.LangPeace.Num || scaned[currentPos].langPeace == Scaner.LangPeace.ID)
            {
                ret.analyseSequence.Add("O(num/ID)");
                ret.nextStartPos = currentPos + 1;
                return ret;
            }
            if (scaned[currentPos].langPeace == Scaner.LangPeace.LeftBracket)
            {
                ret.analyseSequence.Add("O(E)");
                openBracketsCount++;
                CheckResult eRes = E(scaned, currentPos + 1);
                openBracketsCount--;
                ret.analyseSequence.AddRange(eRes.analyseSequence);
                ret.errors.AddRange(eRes.errors);
                if (eRes.correct)
                {
                    currentPos = eRes.nextStartPos;
                    if (currentPos < scaned.Count && scaned[currentPos].langPeace == Scaner.LangPeace.RightBracket)
                    {
                        ret.nextStartPos = currentPos + 1;
                    }
                    else
                    {
                        ret.errors.Add(new ErrorInText(scaned[currentPos - 1].NextSymbol, 1, "Требуется \")\""));
                        ret.nextStartPos = currentPos;
                    }
                }
                else
                {
                    if (currentPos + 1 < scaned.Count && scaned[currentPos + 1].langPeace == Scaner.LangPeace.RightBracket)
                    {
                        ret.nextStartPos = currentPos + 1;
                        ret.errors.Add(new ErrorInText(scaned[currentPos].pos + 1, 1, "Требуется выражение"));
                    }
                    else
                    {
                        ret.errors.Add(new ErrorInText(scaned[currentPos].NextSymbol, 1, "Требуется выражение"));
                        ret.errors.Add(new ErrorInText(scaned[currentPos].NextSymbol, 1, "Требуется \")\""));
                        ret.nextStartPos = currentPos + 1;
                    }
                }
                return ret;
            }

            ret.analyseSequence.Add("O");
            ret.correct = false;
            ret.errors.Add(new ErrorInText(scaned[ret.startPos].pos + 1, 1, "Требуется выражение"));
            ret.nextStartPos = ret.startPos;
            return ret;
        }
    }
}
