using System.Collections.Generic;

namespace Compiler
{
    internal class FortranConstantsFinder
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
            public string constantName, constantValue;
            public bool correct;
            public CheckResult(string constantName, string constantValue)
            {
                errors = new List<ErrorInText>();
                this.constantName = constantName;
                this.constantValue = constantValue;
                correct = true;
            }
        }

        private static int state;

        private static bool Isw(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z') || (symbol >= '0' && symbol <= '9') || symbol == '_')
                return true;
            else
                return false;
        }

        private static bool IsLetter(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z'))
                return true;
            else
                return false;
        }

        private static bool IsDigit(char symbol)
        {
            if ((symbol >= '0' && symbol <= '9'))
                return true;
            else
                return false;
        }
        private static void AddError(int pos, List<ErrorInText> errors, string comment = "Неожиданный символ")
        {
            if (errors.Count == 0)
            {
                errors.Add(new ErrorInText(pos, 1, comment));
                return;
            }
            if (errors[errors.Count - 1].position + errors[errors.Count - 1].length == pos)
            {
                errors[errors.Count - 1] = new ErrorInText(errors[errors.Count - 1].position, errors[errors.Count - 1].length + 1, comment);
            }
            else
            {
                errors.Add(new ErrorInText(pos, 1, comment));
            }
        }

        public static CheckResult CheckString(string input)
        {
            CheckResult checkResult = new CheckResult("", "");
            state = 0;
            for (int i = 0; i <= input.Length; i++)
            {
                switch (state)
                {
                    case 0:
                        {
                            CheckResult result = CheckReal(input, ref i);
                            checkResult.errors.AddRange(result.errors);
                            i--;
                            state = 1;
                            break;
                        }
                    case 1:
                        {
                            if (i >= input.Length)
                            {
                                checkResult.errors.Add(new ErrorInText(i, 0, "Ожидалось \",\""));
                                i--;
                                state = 2;
                                break;
                            }
                            if (input[i] == ',')
                                state = 2;
                            else if (input[i] == ' ')
                                state = 1;
                            else
                            {
                                i--;
                                state = 2;
                                checkResult.errors.Add(new ErrorInText(i, 0, "Ожидалось \",\""));
                            }

                            break;
                        }
                    case 2:
                        {
                            CheckResult result = CheckParameter(input, ref i);
                            checkResult.errors.AddRange(result.errors);
                            i--;
                            state = 3;
                            break;
                        }
                    case 3:
                        {
                            CheckResult result = CheckDoublePoints(input, ref i);
                            checkResult.errors.AddRange(result.errors);
                            i--;
                            state = 16;
                            break;
                        }
                    case 16:
                        {
                            CheckResult result = CheckID(input, ref i);
                            if (result.errors.Count == 1)
                            {
                                if (result.errors[0].comment == "Ожидался идентификатор")
                                    checkResult.constantName = "ID";
                                else
                                    checkResult.constantName = result.constantName;
                            }
                            else
                                checkResult.constantName = result.constantName;
                            checkResult.errors.AddRange(result.errors);
                            i--;
                            state = 19;
                            break;
                        }
                    case 19:
                        {
                            if (i >= input.Length)
                            {
                                checkResult.errors.Add(new ErrorInText(i, 0, "Ожидалось \"=\""));
                                i--;
                                state = 20;
                                break;
                            }
                            if (input[i] == '=')
                                state = 20;
                            else if (input[i] == ' ')
                                state = 19;
                            else
                            {
                                i--;
                                state = 20;
                                checkResult.errors.Add(new ErrorInText(i, 0, "Ожидалось \"=\""));
                            }

                            break;
                        }
                    case 20:
                        {
                            CheckResult result = CheckNumber(input, ref i);
                            if (result.errors.Count == 1)
                            {
                                if (result.errors[0].comment == "Ожидалось число")
                                    checkResult.constantValue = "Number";
                                else
                                    checkResult.constantValue = result.constantValue;
                            }
                            else
                                checkResult.constantValue = result.constantValue;
                            checkResult.errors.AddRange(result.errors);
                            i--;
                            state = 21;
                            break;
                        }
                    case 21:
                        {
                            break;
                        }
                }
            }
            checkResult.correct = state == 21;
            return checkResult;
        }

        public static CheckResult CheckReal(string input, ref int pos)
        {
            int state = 0;
            int start = pos;
            CheckResult result = new CheckResult();
            result.errors = new List<ErrorInText>();
            while (true)
            {
                if (pos == input.Length)
                {
                    result.correct = state == 4;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось real"));
                        result.correct = true;
                    }
                    return result;
                }
                if ((state != 0 && input[pos] == ' ') || input[pos] == ',')
                {
                    {
                        result.correct = state == 4;
                        if (!result.correct)
                        {
                            result.errors.Clear();
                            result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось real"));
                            result.correct = true;
                        }
                        return result;
                    }
                }
                switch (state)
                {
                    case 0: { if (input[pos] == ' ') state = 0; else if (input[pos] == 'r') state = 1; else { AddError(pos, result.errors); } break; }
                    case 1: { if (input[pos] == 'e') state = 2; else { AddError(pos, result.errors); } break; }
                    case 2: { if (input[pos] == 'a') state = 3; else { AddError(pos, result.errors); } break; }
                    case 3: { if (input[pos] == 'l') state = 4; else { AddError(pos, result.errors); } break; }
                    case 4: { AddError(pos, result.errors); break; }
                }
                pos++;
            }
        }
        public static CheckResult CheckParameter(string input, ref int pos)
        {
            int state = 0;
            int start = pos;
            CheckResult result = new CheckResult();
            result.errors = new List<ErrorInText>();
            while (true)
            {
                if (pos == input.Length)
                {
                    result.correct = state == 9;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось parameter"));
                        result.correct = true;
                    }
                    return result;
                }
                if ((state != 0 && input[pos] == ' ') || input[pos] == ':')
                {
                    result.correct = state == 9;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось parameter"));
                        result.correct = true;
                    }
                    return result;
                }
                switch (state)
                {
                    case 0: { if (input[pos] == ' ') state = 0; else if (input[pos] == 'p') state = 1; else { AddError(pos, result.errors); } break; }
                    case 1: { if (input[pos] == 'a') state = 2; else { AddError(pos, result.errors); } break; }
                    case 2: { if (input[pos] == 'r') state = 3; else { AddError(pos, result.errors); } break; }
                    case 3: { if (input[pos] == 'a') state = 4; else { AddError(pos, result.errors); } break; }
                    case 4: { if (input[pos] == 'm') state = 5; else { AddError(pos, result.errors); } break; }
                    case 5: { if (input[pos] == 'e') state = 6; else { AddError(pos, result.errors); } break; }
                    case 6: { if (input[pos] == 't') state = 7; else { AddError(pos, result.errors); } break; }
                    case 7: { if (input[pos] == 'e') state = 8; else { AddError(pos, result.errors); } break; }
                    case 8: { if (input[pos] == 'r') state = 9; else { AddError(pos, result.errors); } break; }
                    case 9: { AddError(pos, result.errors); break; }
                }
                pos++;
            }
        }
        public static CheckResult CheckDoublePoints(string input, ref int pos)
        {
            int state = 0;
            int start = pos;
            CheckResult result = new CheckResult();
            result.errors = new List<ErrorInText>();
            while (true)
            {
                if (pos == input.Length)
                {
                    result.correct = state == 2;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось ::"));
                        result.correct = true;
                    }
                    return result;
                }
                if ((state != 0 && input[pos] == ' ' || IsLetter(input[pos])))
                {
                    result.correct = state == 2;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось ::"));
                        result.correct = true;
                    }
                    return result;
                }
                switch (state)
                {
                    case 0: { if (input[pos] == ' ') state = 0; else if (input[pos] == ':') state = 1; else { AddError(pos, result.errors); } break; }
                    case 1: { if (input[pos] == ':') state = 2; else { AddError(pos, result.errors); } break; }
                    case 2: { AddError(pos, result.errors); break; }
                }
                pos++;
            }
        }
        public static CheckResult CheckID(string input, ref int pos)
        {
            int state = 0;
            int start = pos;
            CheckResult result = new CheckResult();
            result.errors = new List<ErrorInText>();
            string id = "";
            while (true)
            {
                if (pos == input.Length)
                {
                    result.correct = state == 1;
                    result.constantName = id;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидался идентификатор"));
                        result.correct = true;
                    }
                    return result;
                }
                if ((state != 0 && input[pos] == ' ' || input[pos] == '='))
                {
                    result.correct = state == 1;
                    result.constantName = id;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидался идентификатор"));
                        result.correct = true;
                    }
                    return result;
                }
                switch (state)
                {
                    case 0:
                        {
                            if (input[pos] == ' ')
                                state = 0;
                            else if (IsLetter(input[pos]))
                            {
                                id += input[pos];
                                state = 1;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 1:
                        {
                            if (Isw(input[pos]))
                            {
                                id += input[pos];
                                state = 1;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                }
                pos++;
            }
        }
        public static CheckResult CheckNumber(string input, ref int pos)
        {
            int state = 19;
            int start = pos;
            CheckResult result = new CheckResult();
            result.errors = new List<ErrorInText>();
            string number = "";
            while (true)
            {
                if (pos == input.Length)
                {
                    result.correct = state == 21 || state == 22 || state == 25;
                    result.constantValue = number;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось число"));
                        result.correct = true;
                    }
                    return result;
                }
                if ((state != 19 && input[pos] == ' ' || input[pos] == '\n'))
                {
                    result.correct = state == 21 || state == 22 || state == 25;
                    result.constantValue = number;
                    if (!result.correct)
                    {
                        result.errors.Clear();
                        result.errors.Add(new ErrorInText(start, pos - start, "Ожидалось число"));
                        result.correct = true;
                    }
                    return result;
                }
                switch (state)
                {
                    case 19:
                        {
                            if (input[pos] == ' ')
                                state = 19;
                            else if (input[pos] == '-' || input[pos] == '+')
                            {
                                if (input[pos] == '-')
                                    number += input[pos];
                                state = 20;
                            }
                            else if (input[pos] == 'e')
                            {
                                number += input[pos];
                                state = 23;
                            }
                            else if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 21;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 20:
                        {
                            if (input[pos] == ' ')
                                state = 19;
                            else if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 21;
                            }
                            else if (input[pos] == 'e')
                            {
                                number += input[pos];
                                state = 23;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 21:
                        {
                            if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 21;
                            }
                            else if (input[pos] == 'e')
                            {
                                number += input[pos];
                                state = 23;
                            }
                            else if (input[pos] == '.')
                            {
                                number += input[pos];
                                state = 22;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 22:
                        {
                            if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 22;
                            }
                            else if (input[pos] == 'e')
                            {
                                number += input[pos];
                                state = 23;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 23:
                        {
                            if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 25;
                            }
                            else if (input[pos] == '-' || input[pos] == '+')
                            {
                                if (input[pos] == '-')
                                    number += input[pos];
                                state = 24;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 24:
                        {
                            if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 25;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }
                    case 25:
                        {
                            if (IsDigit(input[pos]))
                            {
                                number += input[pos];
                                state = 25;
                            }
                            else
                                AddError(pos, result.errors);
                            break;
                        }

                }
                pos++;
            }
        }
    }
}