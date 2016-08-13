using System;
using System.Runtime.Serialization;

namespace News360.Equation.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException(string input, int position, ParseState state, string message, Exception inner = null)
            : base($"{message}\nExpression:{input}\nPosition: {position} Symbol: '{input[position]}' State: {state}", inner)
        {
            Position = position;
            State = state;
        }
        public int Position { get; set; }
        public ParseState State { get; set; }
    }
}