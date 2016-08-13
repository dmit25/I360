using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using News360.Equation.Data;

namespace News360.Equation.Parsing
{
    public class Parser
    {
        public Data.Equation Parse(string input)
        {
            var pos = 0;
            var state = ParseState.Factor;
            if (string.IsNullOrEmpty(input))
            {
                throw new ParsingException(input, pos, state, "input is null or empty");
            }
            try
            {
                var res = new Data.Equation();
                var context = new Stack<IList<Member>>();
                context.Push(res.LeftPart);
                var buffer = new StringBuilder();
                var current = new Member(0);
                Variable currentVar = null;
                for (pos = 0; pos < input.Length; pos++)
                {
                    var c = input[pos];
                    switch (c)
                    {
                        case ' ':
                            continue;
                        case '=':
                            if (context.Count > 1)
                            {
                                throw new ApplicationException("Unclosed brackets");
                            }
                            current = AddDanglingMember(current, buffer, context, out currentVar);
                            context.Pop();
                            context.Push(res.RightPart);
                            state = ParseState.Factor;
                            break;
                        case '(':
                            switch (state)
                            {
                                case ParseState.Factor:
                                    SetFactorFromBuffer(current, buffer);
                                    current = PushBrackets(current, context, out currentVar, out state);
                                    break;
                                case ParseState.Power:
                                    SetPowerFromBuffer(currentVar, buffer);
                                    current = PushBrackets(current, context, out currentVar, out state);
                                    break;
                                case ParseState.Variable:
                                    current = PushBrackets(current, context, out currentVar, out state);
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        case ')':
                            switch (state)
                            {
                                case ParseState.Power:
                                case ParseState.Factor:
                                    current = AddDanglingMember(current, buffer, context, out currentVar);
                                    context.Pop();
                                    break;
                                case ParseState.Variable:
                                    if (context.Count < 2)
                                    {
                                        throw new ApplicationException("Redundant close bracket");
                                    }
                                    current = NewMember(context, current, out currentVar);
                                    context.Pop();
                                    state = ParseState.Factor;
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        case '+':
                        case '-':
                            switch (state)
                            {
                                case ParseState.Factor:
                                    buffer.Append(c);
                                    break;
                                case ParseState.Variable:
                                    current = NewMember(context, current, out currentVar);
                                    buffer.Append(c);
                                    state = ParseState.Factor;
                                    break;
                                case ParseState.Power:
                                    SetPowerFromBuffer(currentVar, buffer);
                                    current = NewMember(context, current, out currentVar);
                                    state = ParseState.Factor;
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        case '.':
                            if (state == ParseState.Factor)
                            {
                                buffer.Append(c);
                            }
                            else
                            {
                                return Unexpected(c);
                            }
                            break;
                        case '^':
                            switch (state)
                            {
                                case ParseState.Variable:
                                    state = ParseState.Power;
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        default:
                            if (c >= '0' && c <= '9')
                            {
                                switch (state)
                                {
                                    case ParseState.Factor:
                                    case ParseState.Power:
                                        buffer.Append(c);
                                        break;
                                    case ParseState.Variable:

                                        break;
                                    default:
                                        return Unexpected(c);
                                }
                            }
                            else if (c >= 'a' && c <= 'z')
                            {
                                switch (state)
                                {
                                    case ParseState.Factor:
                                        SetFactorFromBuffer(current, buffer);
                                        currentVar = current.AddVariable(c.ToString());
                                        state = ParseState.Variable;
                                        break;
                                    case ParseState.Variable:
                                        currentVar = current.AddVariable(c.ToString());
                                        break;
                                    case ParseState.Power:
                                        break;
                                    default:
                                        return Unexpected(c);
                                }
                            }
                            else
                            {
                                return Unexpected(c);
                            }
                            break;
                    }
                }
                SetFactorFromBuffer(current, buffer, true);
                if (!IsEmpty(current))
                {
                    context.Peek().Add(current);
                }
                return res;
            }
            catch (ApplicationException exc)
            {
                throw new ParsingException(input, pos, state, exc.Message, exc);
            }
        }

        private Member AddDanglingMember(Member current, StringBuilder buffer, Stack<IList<Member>> context, out Variable currentVar)
        {
            SetFactorFromBuffer(current, buffer, true);
            if (!IsEmpty(current))
            {
                current = NewMember(context, current, out currentVar);
            }
            else
            {
                currentVar = null;
            }
            return current;
        }

        private bool IsEmpty(Member current)
        {
            return Math.Abs(current.Factor) < double.Epsilon && current.Vars.Count == 0;
        }

        private void SetPowerFromBuffer(Variable current, StringBuilder buffer)
        {
            if (buffer.Length == 0)
            {
                throw new ApplicationException("Empty power value");
            }
            if (current == null)
            {
                throw new ApplicationException("Undefined behaviour");
            }
            current.Power = int.Parse(buffer.ToString());
            buffer.Clear();
        }

        private static Member PushBrackets(Member current, Stack<IList<Member>> context, out Variable currentVar, out ParseState state)
        {
            var br = new Brackets(current);
            context.Peek().Add(br);
            context.Push(br.Content);
            current = new Member(0);
            currentVar = null;
            state = ParseState.Factor;
            return current;
        }

        private static void SetFactorFromBuffer(Member current, StringBuilder buffer, bool ignoreEmptyBuffer = false)
        {
            if (buffer.Length > 0)
            {
                current.Factor = buffer.Length == 1 && (buffer[0] == '-' || buffer[0] == '+')
                    ? (buffer[0] == '-' ? -1 : +1)
                    : double.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
                buffer.Clear();
            }
            else if (!ignoreEmptyBuffer && current != null && Math.Abs(current.Factor) < double.Epsilon)
            {
                current.Factor = 1;
            }
        }

        private static Member NewMember(Stack<IList<Member>> context, Member current, out Variable currentVar)
        {
            context.Peek().Add(current);
            current = new Member(0);
            currentVar = null;
            return current;
        }

        private static Data.Equation Unexpected(char c) { throw new ApplicationException($"Unexpected symbol '{c}'"); }
    }

    public enum ParseState
    {
        Factor,
        Variable,
        Power,
    }
}