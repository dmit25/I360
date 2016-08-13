using System.Collections.Generic;
using News360.Equation.Parsing;
using NUnit.Framework;

namespace News360.Equation.Tests
{
    [TestFixture]
    public class ParserTests : TestBase
    {
        public static IEnumerable<KeyValuePair<string, Data.Equation>> CorrectCases()
        {
            return new Dictionary<string, Data.Equation>
            {
                { "x^2 + 3.5xy + y = y^2 - xy + y",
                    L(
                        M(1,"x",2),
                        M(+3.5,"x").V("y"),
                        M(+1,"y"))
                    .R(
                        M(1,"y",2),
                        M(-1,"x").V("y"),
                        M(+1,"y"))
                },

                { "x^2 + 3.5x(y + y) = y^2( - xy + y)",
                    L(
                        M(1,"x",2),
                        M(+3.5,"x").B(
                            M(1,"y"),
                            M(+1,"y")))
                    .R( M(1,"y",2).B(
                            M(-1,"x").V("y"),
                            M(+1,"y")))
                },

                { "x^2y^-2 + 3.5(y + x^2 + 1) = 0",
                    L(
                        M(1,"x",2).V("y",-2),
                        M(+3.5,"").B(
                            M(1,"y"),
                            M(+1,"x",2),
                            M(1,"")))
                    .R()
                },


                { "0 = -25",
                    L()
                    .R(
                        M(-25,""))
                },

                { "xxyyyy = -25",
                    L(M(1,"x",2).V("y",4))
                    .R(
                        M(-25,""))
                },

                { "xx = yyyy",
                    L(M(1,"x",2))
                    .R(
                        M(1,"y",4))
                },

                { "x^5 = yyyy^3",
                    L(M(1,"x",5))
                    .R(
                        M(1,"y",6))
                },

                { "x^5 = yy^3",
                    L(M(1,"x",5))
                    .R(
                        M(1,"y",4))
                },

                { "x^5 = yy^-1",
                    L(M(1,"x",5))
                    .R(
                        M(1,"y",0))
                },

                { "x^2 + 3.5x(yx + y) = -y^2( - xy + 3(x - y)) + 1",
                    L(
                        M(1,"x",2),
                        M(3.5,"x").B(
                            M(1,"y").V("x"),
                            M(+1,"y")
                        ))
                    .R(
                        M(-1,"y",2).B(
                            M(-1,"x").V("y"),
                            M(+3,"").B(
                                M(1,"x"),
                                M(-1,"y"))),
                        M(1,"")
                        )
                },
                {"xxyyyy - 0 = -25 + 0",
                    L(
                        M(1,"x",2).V("y",4))
                    .R(
                        M(-25,""))
                },
                {"xxyyyy - 0.5 = -25 + 0.25",
                    L(
                        M(1,"x",2).V("y",4),
                        M(-0.5,""))
                    .R(
                        M(-25,""),
                        M(+0.25,""))
                }
            };
        }

        private static readonly Parser _Parser = new Parser();

        [Test, TestCaseSource(nameof(CorrectCases))]
        public void CanParseEquations(KeyValuePair<string, Data.Equation> test)
        {
            var res = _Parser.Parse(test.Key);
            AssertAreEqual(test.Value, res);
        }

        public static IEnumerable<string> IncorrectCases()
        {
            return new
            []{
                //incorrect naming (by design)
                "x0^5 = yy^-1",
                "x^5 = yy^--1",
                "x^5 ",
                "x^5 = yy^--1 = x",
                //[= -] <=> [= -0]
                //"x^5 = -", 
                "x^y = -"
            };
        }

        [Test, TestCaseSource(nameof(IncorrectCases))]
        public void CantParseIncorrectEquations(string test)
        {
            Assert.That(() => { _Parser.Parse(test); }, Throws.Exception);
        }
    }
}
