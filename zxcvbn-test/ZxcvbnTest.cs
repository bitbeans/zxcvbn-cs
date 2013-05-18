using System;
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
    }
}
