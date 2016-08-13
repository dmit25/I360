using System.Collections.Generic;
using News360.Equation.Canonization;
using News360.Equation.Parsing;
using NUnit.Framework;

namespace News360.Equation.Tests
{
    [TestFixture]
    public class CanonizerTests : TestBase
    {
        public static IEnumerable<KeyValuePair<string, Data.Equation>> CorrectCases()
        {
            return new Dictionary<string, Data.Equation>
            {
                { "x^2 + 3.5xy + y = y^2 - xy + y",
                //x^2 - y^2 + 4.5xy = 0",
                    L(
                        M(1,"x",2),
                        M(-1,"y",2),
                        M(+4.5,"x").V("y"))
                    .R(
                        M(0,""))
                },

                { "x^2 + 3.5x(y + y) = y^2(-xy + y)",
                //xy^3 - y^3 + x^2 + 7xy = 0
                    L(
                        M(1,"x").V("y",3),
                        M(-1,"y",3),
                        M(+1,"x",2),
                        M(+7,"x").V("y"))
                    .R(
                        M(0,""))
                },

                { "x^2 + 3.5x(yx + y) = -y^2(-xy + 3(x - y)) + 1",
                //-xy^3 - 3y^3 + 3.5x^2y + 3xy^2 + x^2 + 3.5y^2 - 1 = 0
                    L(
                        M(-3,"x").V("y",3),
                        M(-3,"y",3),
                        M(+3.5,"x",2).V("y"),
                        M(+3,"x").V("y",2),
                        M(+1,"x",2),
                        M(+3.5,"y",2),
                        M(-1,"")
                        )
                    .R(
                        M(0,"")
                        )
                },
            };
        }

        private static readonly Parser _Parser = new Parser();
        private static readonly Canonizer _Canonizer = new Canonizer();

        [Test, TestCaseSource(nameof(CorrectCases))]
        public void CanParseEquations(KeyValuePair<string, Data.Equation> test)
        {
            var res = _Canonizer.Canonize(_Parser.Parse(test.Key));
            AssertAreEqual(test.Value, res);
        }
    }
}