using System.Linq;
using System.Text;
using News360.Equation.Data;
using News360.Equation.Rendering;

namespace News360.Equation.Tests
{
    public static class Extensions
    {
        private static readonly Renderer _Renderer = new Renderer();

        public static string Render(this Data.Equation equation) { return _Renderer.Render(equation); }

        public static string Render(this Data.Member member)
        {
            var sb = new StringBuilder();
            _Renderer.Render(sb, member);
            return sb.ToString();
        }

        public static string Render(this Variable v)
        {
            var sb = new StringBuilder();
            _Renderer.Render(sb, v);
            return sb.ToString();
        }

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