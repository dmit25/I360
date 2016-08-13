using System.Collections.Generic;

namespace News360.Equation.Data
{
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
        public Member(double factor) { Factor = factor; }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public Member(double factor, Variable var)
        {
            Factor = factor;
            Vars.Add(var.Name, var);
        }

        /// <summary>
        /// Adds another variable into this member
        /// </summary>
        /// <param name="name"></param>
        /// <param name="power"></param>
        public Member AddVariable(string name, int power = 1)
        {
            Variable existing;
            if (!Vars.TryGetValue(name, out existing))
            {
                Vars.Add(name, new Variable(name, power));
            }
            else
            {
                existing.Power += power;
            }
            return this;
        }
    }
}