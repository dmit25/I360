using System.Collections.Generic;
using News360.Equation.Rendering;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace News360.Equation.Tests
{
    [TestFixture]
    public class RendererTests : TestBase
    {
        public static IEnumerable<KeyValuePair<string, Data.Equation>> Cases()
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

                { "x^2 + 3.5x(y + y) = y^2(-xy + y)",
                    L(  M(1,"x",2),
                        M(+3.5,"x").B(
                            M(1,"y"),
                            M(+1,"y")))
                    .R( M(1,"y",2).B(
                            M(-1,"x").V("y"),
                            M(+1,"y")))
                },

                { "x^2 + 3.5x(xy + y) = -y^2(-xy + 3(x - y)) + 1",
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
            };
        }

        private static readonly Renderer _Renderer = new Renderer();

        [Test, TestCaseSource(nameof(Cases))]
        public void CanRenderEquations(KeyValuePair<string, Data.Equation> test)
        {
            Assert.That(_Renderer.Render(test.Value), Is.EqualTo(test.Key));
        }
    }
}