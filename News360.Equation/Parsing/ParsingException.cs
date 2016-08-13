using System;
using System.Runtime.Serialization;

namespace News360.Equation.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException(int position, ParseState state, string message, Exception inner = null)
            : base($"{position}-{state}: {message}", inner)
        {
            Position = position;
            State = state;
        }
        public int Position { get; set; }
        public ParseState State { get; set; }
    }
}