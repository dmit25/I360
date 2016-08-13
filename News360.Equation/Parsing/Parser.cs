using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using News360.Equation.Data;

namespace News360.Equation.Parsing
{
    public class Parser
    {
        public Data.Equation Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ParsingException(input, 0, ParseState.Factor, "input is null or empty");
            }
            var ctx = new Context();
            var equalitySignWasFound = false;
            try
            {
                var res = new Data.Equation();
                ctx.Stack.Push(res.LeftPart);
                for (; ctx.Pos < input.Length; ctx.Pos++)
                {
                    var c = input[ctx.Pos];
                    switch (c)
                    {
                        case ' ':
                            continue;
                        case '=':
                            if (equalitySignWasFound)
                            {
                                throw new ApplicationException("Incorrect expression");
                            }
                            equalitySignWasFound = true;
                            if (ctx.Stack.Count > 1)
                            {
                                throw new ApplicationException("Unclosed brackets");
                            }
                            AppendDanglingMember(ctx);
                            ctx.Stack.Pop();
                            ctx.Stack.Push(res.RightPart);
                            ctx.State = ParseState.Factor;
                            break;
                        case '(':
                            switch (ctx.State)
                            {
                                case ParseState.Factor:
                                    SetFactorFromBuffer(ctx);
                                    PushBrackets(ctx);
                                    break;
                                case ParseState.Power:
                                    SetPowerFromBuffer(ctx);
                                    PushBrackets(ctx);
                                    break;
                                case ParseState.Variable:
                                    PushBrackets(ctx);
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        case ')':
                            if (ctx.Stack.Count < 2)
                            {
                                throw new ApplicationException("Redundant close bracket");
                            }
                            AppendDanglingMember(ctx);
                            ctx.Stack.Pop();
                            ctx.State = ParseState.Factor;
                            break;
                        case '+':
                        case '-':
                            switch (ctx.State)
                            {
                                case ParseState.Factor:
                                    if (ctx.Buffer.Length > 0)
                                    {
                                        AppendDanglingMember(ctx);
                                    }
                                    ctx.Buffer.Append(c);
                                    break;
                                case ParseState.Variable:
                                    StartNewMember(ctx);
                                    ctx.Buffer.Append(c);
                                    ctx.State = ParseState.Factor;
                                    break;
                                case ParseState.Power:
                                    //handling negative power
                                    if (ctx.Buffer.Length == 1
                                        && c == '-'
                                        && ctx.Buffer[0] == '^')
                                    {
                                        ctx.Buffer.Append(c);
                                    }
                                    else
                                    {
                                        SetPowerFromBuffer(ctx);
                                        StartNewMember(ctx);
                                        ctx.Buffer.Append(c);
                                        ctx.State = ParseState.Factor;
                                    }
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        case '.':
                            if (ctx.State == ParseState.Factor)
                            {
                                ctx.Buffer.Append(c);
                            }
                            else
                            {
                                return Unexpected(c);
                            }
                            break;
                        case '^':
                            switch (ctx.State)
                            {
                                case ParseState.Variable:
                                    ctx.Buffer.Append(c);
                                    ctx.State = ParseState.Power;
                                    break;
                                default:
                                    return Unexpected(c);
                            }
                            break;
                        default:
                            if (c >= '0' && c <= '9')
                            {
                                switch (ctx.State)
                                {
                                    case ParseState.Factor:
                                    case ParseState.Power:
                                        ctx.Buffer.Append(c);
                                        break;
                                    default:
                                        return Unexpected(c);
                                }
                            }
                            else if (c >= 'a' && c <= 'z')
                            {
                                switch (ctx.State)
                                {
                                    case ParseState.Factor:
                                        SetFactorFromBuffer(ctx);
                                        ctx.Var = ctx.Current.AddVariable(c.ToString());
                                        ctx.State = ParseState.Variable;
                                        break;
                                    case ParseState.Variable:
                                        ctx.Var = ctx.Current.AddVariable(c.ToString());
                                        break;
                                    case ParseState.Power:
                                        SetPowerFromBuffer(ctx);
                                        ctx.Var = ctx.Current.AddVariable(c.ToString());
                                        ctx.State = ParseState.Variable;
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
                AppendDanglingMember(ctx);
                if (!equalitySignWasFound)
                {
                    throw new ApplicationException("Incorrect expression");
                }
                return res;
            }
            catch (Exception exc)
            {
                throw new ParsingException(input, ctx.Pos, ctx.State, exc.Message, exc);
            }
        }

        private void AppendDanglingMember(Context ctx)
        {
            if (ctx.State == ParseState.Power)
            {
                SetPowerFromBuffer(ctx);
            }
            else if (ctx.State == ParseState.Factor)
            {
                SetFactorFromBuffer(ctx, true);
            }
            if (!CurrentIsEmpty(ctx))
            {
                StartNewMember(ctx);
            }
            else
            {
                ctx.Var = null;
            }
        }

        private bool CurrentIsEmpty(Context ctx)
        {
            return Math.Abs(ctx.Current.Factor) < double.Epsilon && ctx.Current.Vars.Count == 0;
        }

        private void SetPowerFromBuffer(Context ctx)
        {
            if (ctx.Buffer.Length == 0)
            {
                throw new ApplicationException("Empty power value");
            }
            if (ctx.Current == null)
            {
                throw new ApplicationException("Undefined behaviour");
            }
            var power = int.Parse(
                ctx.Buffer[0] == '^'
                ? ctx.Buffer.ToString(1, ctx.Buffer.Length - 1)
                : ctx.Buffer.ToString());
            ctx.Var.Power = ctx.Var.Power == 1
                ? power
                : ctx.Var.Power + power - 1;
            ctx.Buffer.Clear();
        }

        private static void PushBrackets(Context ctx)
        {
            var br = new Brackets(ctx.Current);
            ctx.Stack.Peek().Add(br);
            ctx.Stack.Push(br.Content);
            ctx.Current = new Member(0);
            ctx.Var = null;
            ctx.State = ParseState.Factor;
        }

        private static void SetFactorFromBuffer(Context ctx, bool ignoreEmptyBuffer = false)
        {
            if (ctx.Buffer.Length > 0)
            {
                ctx.Current.Factor = ctx.Buffer.Length == 1
                    && (ctx.Buffer[0] == '-'
                    || ctx.Buffer[0] == '+')
                    ? (ctx.Buffer[0] == '-' ? -1 : +1)
                    : double.Parse(ctx.Buffer.ToString(), CultureInfo.InvariantCulture);
                ctx.Buffer.Clear();
            }
            else if (!ignoreEmptyBuffer
                && ctx.Current != null
                && Math.Abs(ctx.Current.Factor) < double.Epsilon)
            {
                ctx.Current.Factor = 1;
            }
        }

        private static void StartNewMember(Context ctx)
        {
            ctx.Stack.Peek().Add(ctx.Current);
            ctx.Current = new Member(0);
            ctx.Var = null;
        }

        private static Data.Equation Unexpected(char c) { throw new ApplicationException($"Unexpected symbol '{c}'"); }

        private class Context
        {
            /// <summary>
            /// Parents stack
            /// </summary>
            public readonly Stack<IList<Member>> Stack = new Stack<IList<Member>>();
            /// <summary>
            /// Inner string buffer (parsing ints and doubles from it)
            /// </summary>
            public readonly StringBuilder Buffer = new StringBuilder();
            /// <summary>
            /// Current expression member
            /// </summary>
            public Member Current = new Member(0);
            /// <summary>
            /// Current member's variable
            /// </summary>
            public Variable Var = null;
            /// <summary>
            /// Current state;
            /// </summary>
            public ParseState State = ParseState.Factor;
            /// <summary>
            /// Current position
            /// </summary>
            public int Pos = 0;
        }
    }

    public enum ParseState
    {
        Factor,
        Variable,
        Power,
    }
}