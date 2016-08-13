using System.Collections.Generic;

namespace News360.Equation.Data
{
    public class Equation
    {
        private readonly List<Member> _leftPart = new List<Member>();
        private readonly List<Member> _rightPart = new List<Member>();
        public List<Member> LeftPart { get { return _leftPart; } }
        public List<Member> RightPart { get { return _rightPart; } }
    }
}