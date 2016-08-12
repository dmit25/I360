using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace News360.Equation.Tests
{
    public static class Ext
    {
        public static Variable V(this string name, double factor = 1.0, int power = 1)
        {
            return new Variable { Factor = factor, Name = name, Power = power };
        }

        public static Brackets B(this string name, double factor, params Member[] members)
        {
            return new Brackets
            {
                Name = name,
                Factor = factor,
                Content = members.ToList()
            };
        }

        public static Equation L(params Member[] members)
        {
            return new Equation { LeftPart = members.ToList() };
        }

        public static Equation R(this Equation equation, params Member[] members)
        {
            equation.RightPart = members.ToList();
            return equation;
        }
    }

    [TestFixture]
    public class ParserTests
    {
        public IEnumerable<KeyValuePair<string, Equation>> Cases()
        {
            return new Dictionary<string, Equation>
            {
                { "x^2 + 3.5xy + y = y^2 - xy + y",
                    Ext.L("x".V(1,2),"xy".V(3.5),"y".V())
                    .R("y".V(1,2),"xy".V(-1),"y".V())
                },

                { "x^2 + 3.5x(y + y) = y^2( - xy + y)",
                    Ext.L("x".V(1,2),"x".B(3.5,"y".V(),"y".V()))
                    .R("y".V(1,2),"xy".V(-1),"y".V())
                },
            };
        }

        [Test, TestCaseSource(nameof(Cases))]
        public void NAME()
        {

        }
    }
}
