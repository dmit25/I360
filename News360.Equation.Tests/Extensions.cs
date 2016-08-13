using System.Linq;
using News360.Equation.Data;

namespace News360.Equation.Tests
{
    public static class Extensions
    {
        public static Member V(this Member member, string name, int power = 1)
        {
            member.AddVariable(name, power);
            return member;
        }

        public static Brackets B(this Member source, params Member[] members)
        {
            var res = new Brackets(source);

            foreach (var member in members)
            {
                res.AddMember(member);
            }
            return res;
        }

        public static Data.Equation R(this Data.Equation equation, params Member[] members)
        {
            equation.RightPart.AddRange(members);
            return equation;
        }
    }
}