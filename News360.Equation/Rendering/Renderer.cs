using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using News360.Equation.Data;

namespace News360.Equation.Rendering
{
    public class Renderer
    {
        public string Render(Data.Equation equation)
        {
            var result = new StringBuilder();
            Render(result, equation.LeftPart);
            result.Append(" = ");
            Render(result, equation.RightPart);
            return result.ToString().Trim();
        }

        public void Render(StringBuilder container, List<Member> leftPart)
        {
            Member prev = null;
            if (leftPart.Count == 0)
            {
                container.Append("0");
            }
            foreach (var member in leftPart)
            {
                Render(container, member, prev);
                prev = member;
            }
        }

        public void Render(StringBuilder container, Member member, Member prev = null)
        {
            var sign = member.Factor > 0 ? " + " : (prev == null ? "-" : " - ");
            if (member.Factor < 0 || prev != null)
            {
                container.Append(sign);
            }
            if (Math.Abs(Math.Abs(member.Factor) - 1) > double.Epsilon || member.Vars.Count == 0)
            {
                container.Append(Math.Abs(member.Factor).ToString(CultureInfo.InvariantCulture));
            }
            foreach (var pair in member.Vars.OrderBy(v => v.Key))
            {
                Render(container, pair.Value);
            }

            var br = member as Brackets;
            if (br != null)
            {
                container.Append("(");
                Render(container, br.Content);
                container.Append(")");
            }
        }

        public void Render(StringBuilder container, Variable v)
        {
            container.Append(v.Name);
            if (v.Power != 1)
            {
                container.Append('^').Append(v.Power);
            }
        }
    }
}