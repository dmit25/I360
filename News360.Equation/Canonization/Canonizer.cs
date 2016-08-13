using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using News360.Equation.Data;

namespace News360.Equation.Canonization
{
    public class Canonizer
    {
        public Data.Equation Canonize(Data.Equation equation)
        {
            var res = new Data.Equation();
            var acc = new Dictionary<string, Member>();
            foreach (var member in equation.LeftPart)
            {
                AddToDict(acc, member);
            }
            foreach (var member in equation.RightPart)
            {
                member.Factor = member.Factor * -1;
                AddToDict(acc, member);
            }
            res.LeftPart.AddRange(GetMembers(acc));
            return res;
        }

        private IEnumerable<Member> GetMembers(Dictionary<string, Member> acc)
        {
            return acc.Where(p => Math.Abs(p.Value.Factor) > double.Epsilon)
                .OrderByDescending(p => p.Value.Vars.Sum(v => v.Value.Power))
                .ThenBy(p => p.Key)
                .Select(p => p.Value);
        }

        private void AddToDict(Dictionary<string, Member> acc, Member member)
        {
            var br = member as Brackets;
            if (br != null)
            {
                AddToDict(acc, br.Content, br);
            }
            else
            {
                Member value;
                var key = GetKey(member);
                if (string.IsNullOrEmpty(key))
                {
                    member.Vars.Clear();
                }
                if (acc.TryGetValue(key, out value))
                {
                    value.Factor += member.Factor;
                }
                else
                {
                    acc.Add(key, member);
                }
            }
        }

        private void AddToDict(Dictionary<string, Member> acc, List<Member> content, Brackets br)
        {
            foreach (var member in content)
            {
                member.Factor *= br.Factor;
                foreach (var pair in br.Vars)
                {
                    member.AddVariable(pair.Value.Name, pair.Value.Power);
                }
                AddToDict(acc, member);
            }

        }

        private string GetKey(Member member)
        {
            if (member.Vars.Count == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var pair in member.Vars.OrderBy(p => p.Key))
            {
                //aka constant
                if (pair.Value.Power == 0)
                {
                    continue;
                }
                sb.Append(pair.Key);
                if (pair.Value.Power != 1)
                {
                    sb.Append("^").Append(pair.Value.Power.ToString());
                }
            }
            return sb.ToString();
        }
    }
}