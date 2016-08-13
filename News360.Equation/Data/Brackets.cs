using System.Collections.Generic;

namespace News360.Equation.Data
{
    public class Brackets : Member
    {
        public List<Member> Content { get; set; }

        /// <summary>
        /// Creates new brackets out of equation member
        /// </summary>
        /// <param name="source"></param>
        public Brackets(Member source)
        {
            Factor = source.Factor;
            Vars = source.Vars;
            Content = new List<Member>();
        }

        /// <summary>
        /// Adds another member inside brackets
        /// </summary>
        /// <param name="member"></param>
        public Brackets AddMember(Member member)
        {
            Content.Add(member);
            return this;
        }
    }
}