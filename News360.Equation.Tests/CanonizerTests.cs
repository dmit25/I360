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
                        M(+4.5,"x").V("y"),
                        M(-1,"y",2))
                    .R()
                },

                { "x^2 + 3.5x(y + y) = y^2(-xy + y)",
                //xy^3 - y^3 + x^2 + 7xy = 0
                    L(
                        M(1,"x").V("y",3),
                        M(-1,"y",3),
                        M(+1,"x",2),
                        M(+7,"x").V("y"))
                    .R()
                },

                { "x^2 + 3.5x(yx + y) = -y^2(-xy + 3(x - y)) + 1",
                //-xy^3 + 3.5x^2y + 3xy^2 - 3y^3 + x^2 + 3.5xy - 1 = 0
                    L(
                        M(-1,"x").V("y",3),
                        M(+3.5,"x",2).V("y"),
                        M(+3,"x").V("y",2),
                        M(-3,"y",3),
                        M(+1,"x",2),
                        M(+3.5,"x").V("y"),
                        M(-1,"")
                        )
                    .R()
                },
                { "xxyyyy - 0 = -25 + 0",
                //x^2y^4 + 25 = 0
                    L(
                        M(1,"x",2).V("y",4),
                        M(+25,""))
                    .R()
                },

                { "xx = yyyy",
                    L(
                        M(-1,"y",4),
                        M(1,"x",2))
                    .R()
                },

                { "x^5 = yyyy^3",
                    L(
                        M(-1,"y",6),
                        M(1,"x",5))
                    .R()
                },

                { "x^5 = yy^3",
                //x^5 - y^4 = 0
                    L(
                        M(1,"x",5),
                        M(-1,"y",4))
                    .R()
                },

                { "x^5 = yy^-1",
                //x^5 - 1 = 0
                    L(
                        M(1,"x",5),
                        M(-1,""))
                    .R()
                },
            };
        }

        private static readonly Parser _Parser = new Parser();
        private static readonly Canonizer _Canonizer = new Canonizer();

        [Test, TestCaseSource(nameof(CorrectCases))]
        public void CanCanonizeEquations(KeyValuePair<string, Data.Equation> test)
        {
            var res = _Canonizer.Canonize(_Parser.Parse(test.Key));
            AssertAreEqual(test.Value, res);
        }
    }
}