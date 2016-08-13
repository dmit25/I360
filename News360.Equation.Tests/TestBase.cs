using System.Collections.Generic;
using News360.Equation.Data;
using NUnit.Framework;

namespace News360.Equation.Tests
{
    public class TestBase
    {
        public static Member M(double factor, string name, int power = 1)
        {
            return new Member(factor, string.IsNullOrEmpty(name) ? null : new Variable(name, power));
        }

        public static Data.Equation L(params Member[] members)
        {
            var res = new Data.Equation();
            res.LeftPart.AddRange(members);
            return res;
        }


        protected void AssertAreEqual(Data.Equation expected, Data.Equation actual)
        {
            Assert.That(actual, Is.Not.Null);
            AssertAreEqual(expected.LeftPart, actual.LeftPart);
            AssertAreEqual(expected.RightPart, actual.RightPart);
        }

        protected void AssertAreEqual(IList<Member> expected, IList<Member> actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.All.Not.Null);
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Members count");
            for (int index = 0; index < expected.Count; index++)
            {
                AssertAreEqual(expected[index], actual[index]);
            }
        }

        private void AssertAreEqual(Member expected, Member actual)
        {
            Assert.That(actual.Factor, Is.EqualTo(expected.Factor), "Factor");
            Assert.That(actual.Vars.Count, Is.EqualTo(expected.Vars.Count), "Variables");
            foreach (var pair in expected.Vars)
            {
                Assert.That(actual.Vars, Contains.Key(pair.Key));
                AssertAreEqual(pair.Value, actual.Vars[pair.Key]);
            }
        }

        private void AssertAreEqual(Variable expected, Variable actual)
        {
            Assert.That(actual.Name, Is.EqualTo(expected.Name), "Name");
            Assert.That(actual.Power, Is.EqualTo(expected.Power), "Power");
        }
    }
}