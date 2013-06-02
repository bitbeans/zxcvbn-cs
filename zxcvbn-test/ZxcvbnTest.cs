using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace zxcvbn_test
{
    [TestClass]
    public class ZxcvbnTest
    {
        private static string[] testPasswords = new string[] {
			"zxcvbn",
			"qwER43@!",
			"Tr0ub4dour&3",
			"correcthorsebatterystaple",
			"coRrecth0rseba++ery9.23.2007staple$",
			"D0g..................",
			"abcdefghijk987654321",
			"neverforget13/3/1997",
			"1qaz2wsx3edc",
			"temppass22",
			"briansmith",
			"briansmith4mayor",
			"password1",
			"viking",
			"thx1138",
			"ScoRpi0ns",
			"do you know",
			"ryanhunter2000",
			"rianhunter2000",
			"asdfghju7654rewq",
			"AOEUIDHG&*()LS_",
			"12345678",
			"defghi6789",
			"rosebud",
			"Rosebud",
			"ROSEBUD",
			"rosebuD",
			"ros3bud99",
			"r0s3bud99",
			"R0$38uD99",
			"verlineVANDERMARK",
			"eheuczkqyq",
			"rWibMFACxAUGZmxhVncy",
			"Ba9ZyWABu99[BK#6MBgbH88Tofv)vs$w"
        };

        [TestMethod]
        public void RunAllTestPasswords()
        {
            foreach (var password in testPasswords)
            {
                var result = Zxcvbn.Zxcvbn.MatchPassword(password);
            }
        }

        [TestMethod]
        public void RunAllTestPasswordsWithNullMatcher()
        {
            //TODO: Make this test do something useful?

            var zxc = new Zxcvbn.Zxcvbn(new Zxcvbn.DefaultMatcherFactory(new Zxcvbn.Matcher.IMatcher[]{ new Zxcvbn.Matcher.NullMatcher() }));

            foreach (var password in testPasswords)
            {
                var result = zxc.GetPasswordMatches(password);
            }
        }

        [TestMethod]
        public void BruteForceCardinalityTest()
        {
            Assert.AreEqual(26, Zxcvbn.PasswordScoring.PasswordCardinality("asdf"));
            Assert.AreEqual(26, Zxcvbn.PasswordScoring.PasswordCardinality("ASDF"));
            Assert.AreEqual(52, Zxcvbn.PasswordScoring.PasswordCardinality("aSDf"));
            Assert.AreEqual(10, Zxcvbn.PasswordScoring.PasswordCardinality("124890"));
            Assert.AreEqual(62, Zxcvbn.PasswordScoring.PasswordCardinality("aS159Df"));
            Assert.AreEqual(33, Zxcvbn.PasswordScoring.PasswordCardinality("!@<%:{$:#<@}{+&)(*%"));
            Assert.AreEqual(100, Zxcvbn.PasswordScoring.PasswordCardinality("©"));
            Assert.AreEqual(95, Zxcvbn.PasswordScoring.PasswordCardinality("ThisIs@T3stP4ssw0rd!"));
        }

        [TestMethod]
        public void TimeDisplayStrings()
        {
            // Note that the time strings should be + 1
            Assert.AreEqual("11 minutes", Zxcvbn.Utility.DisplayTime(60 * 10));
            Assert.AreEqual("2 days", Zxcvbn.Utility.DisplayTime(60 * 60 * 24));
            Assert.AreEqual("17 years", Zxcvbn.Utility.DisplayTime(60 * 60 * 24 * 365 * 15.4));
        }

        [TestMethod]
        public void RepeatMatcher()
        {
            var repeat = new Zxcvbn.Matcher.RepeatMatcher();

            var res = repeat.MatchPassword("aasdffff");
            Assert.AreEqual(2, res.Count());

            var m1 = res.ElementAt(0);
            Assert.AreEqual(0, m1.i);
            Assert.AreEqual(1, m1.j);
            Assert.AreEqual("aa", m1.Token);

            var m2 = res.ElementAt(1);
            Assert.AreEqual(4, m2.i);
            Assert.AreEqual(7, m2.j);
            Assert.AreEqual("ffff", m2.Token);


            res = repeat.MatchPassword("asdf");
            Assert.AreEqual(0, res.Count());
        }

        [TestMethod]
        public void SequenceMatcher()
        {
            var seq = new Zxcvbn.Matcher.SequenceMatcher();

            var res = seq.MatchPassword("abcd");
            Assert.AreEqual(1, res.Count());
            var m1 = res.First();
            Assert.AreEqual(0, m1.i);
            Assert.AreEqual(3, m1.j);
            Assert.AreEqual("abcd", m1.Token);

            res = seq.MatchPassword("asdfabcdhujzyxwhgjj");
            Assert.AreEqual(2, res.Count());

            m1 = res.ElementAt(0);
            Assert.AreEqual(4, m1.i);
            Assert.AreEqual(7, m1.j);
            Assert.AreEqual("abcd", m1.Token);

            var m2 = res.ElementAt(1);
            Assert.AreEqual(11, m2.i);
            Assert.AreEqual(14, m2.j);
            Assert.AreEqual("zyxw", m2.Token);

            res = seq.MatchPassword("dfsjkhfjksdh");
            Assert.AreEqual(0, res.Count());
        }

        [TestMethod]
        public void DigitsRegexMatcher()
        {
            var re = new Zxcvbn.Matcher.RegexMatcher("\\d{3,}");

            var res = re.MatchPassword("abc123def");
            Assert.AreEqual(1, res.Count());
            var m1 = res.First();
            Assert.AreEqual(3, m1.i);
            Assert.AreEqual(5, m1.j);
            Assert.AreEqual("123", m1.Token);

            res = re.MatchPassword("123456789a12345b1234567");
            Assert.AreEqual(3, res.Count());
            var m3 = res.ElementAt(2);
            Assert.AreEqual("1234567", m3.Token);

            res = re.MatchPassword("12");
            Assert.AreEqual(0, res.Count());

            res = re.MatchPassword("dfsdfdfhgjkdfngjl");
            Assert.AreEqual(0, res.Count());
        }

        [TestMethod]
        public void DateMatcher()
        {
            var dm = new Zxcvbn.Matcher.DateMatcher();

            var res = dm.MatchPassword("123456");

            res = dm.MatchPassword("12345678");
        }
    }
}
