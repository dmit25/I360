using System.Collections.Generic;
using System.Diagnostics;

namespace News360.Equation.Data
{
    [DebuggerDisplay("{Factor}[{Vars.Count}]")]
    public class Member
    {
        public double Factor { get; set; }
        public Dictionary<string, Variable> Vars { get; set; }


        public Member()
        {
            Vars = new Dictionary<string, Variable>();
        }

        /// <summary>
        /// Constant initialization
        /// </summary>
        /// <param name="factor"></param>
        public Member(double factor) : this() { Factor = factor; }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public Member(double factor, Variable var = null) : this()
        {
            Factor = factor;
            if (var != null)
            {
                Vars.Add(var.Name, var);
            }
        }

        /// <summary>
        /// Adds another variable into this member
        /// </summary>
        /// <param name="name"></param>
        /// <param name="power"></param>
        public Variable AddVariable(string name, int power = 1)
        {
            Variable value;
            if (!Vars.TryGetValue(name, out value))
            {
                value = new Variable(name, power);
                Vars.Add(name, value);
            }
            else
            {
                value.Power += power;
            }
            return value;
        }
    }
}