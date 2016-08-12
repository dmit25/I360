using System.Collections.Generic;

namespace News360.Equation
{
    public class Equation
    {
        public List<Member> LeftPart { get; set; }
        public List<Member> RightPart { get; set; }
    }

    public abstract class Member
    {
        public double Factor { get; set; }
        public string Name { get; set; }
    }

    public class Brackets:Member
    {
        public List<Member> Content { get; set; }
    }

    public class Variable : Member
    {
        public int Power { get; set; }
    }

    public class Parser
    {
        public Equation Parse(string input)
        {
            return new Equation();
        }
    }

    public class Canonizer
    {
        public void Canonize(ref Equation equation)
        {

        }
    }

    public class Renderer
    {
        public string Render(Equation equation) { return string.Empty; }
    }
}