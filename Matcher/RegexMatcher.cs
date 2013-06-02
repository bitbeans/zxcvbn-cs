using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zxcvbn.Matcher
{
    public class RegexMatcher : IMatcher
    {
        Regex matchRegex;
        string matcherName;
        int cardinality;

        public RegexMatcher(string pattern, int cardinality, string matcherName = "regex") : this(new Regex(pattern), cardinality, matcherName)
        {
        }

        public RegexMatcher(Regex matchRegex, int cardinality, string matcherName = "regex")
        {
            this.matchRegex = matchRegex;
            this.matcherName = matcherName;
            this.cardinality = cardinality;
        }

        public IEnumerable<Match> MatchPassword(string password)
        {
            var reMatches = matchRegex.Matches(password);

            var pwMatches = new List<Match>();

            foreach (System.Text.RegularExpressions.Match rem in reMatches)
            {
                pwMatches.Add(new Match()
                {
                    Pattern = matcherName,
                    i = rem.Index,
                    j = rem.Index + rem.Length - 1,
                    Token = password.Substring(rem.Index, rem.Length),
                    Cardinality = cardinality,
                    Entropy = Math.Log(cardinality, 2)
                });
            }

            return pwMatches;
        }
    }
}
