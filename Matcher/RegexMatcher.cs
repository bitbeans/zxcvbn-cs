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
        bool perCharCardinality;

        public RegexMatcher(string pattern, int cardinality, bool perCharCardinality = true, string matcherName = "regex") 
            : this(new Regex(pattern), cardinality, perCharCardinality, matcherName)
        {
        }

        public RegexMatcher(Regex matchRegex, int cardinality, bool perCharCardinality, string matcherName = "regex")
        {
            this.matchRegex = matchRegex;
            this.matcherName = matcherName;
            this.cardinality = cardinality;
            this.perCharCardinality = perCharCardinality;
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
                    Entropy = Math.Log((perCharCardinality ? Math.Pow(cardinality, rem.Length) : cardinality), 2) // Raise cardinality to length when giver per character
                });
            }

            return pwMatches;
        }
    }
}
