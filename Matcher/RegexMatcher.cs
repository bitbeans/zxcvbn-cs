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

        public RegexMatcher(string pattern, string matcherName = "regex") : this(new Regex(pattern), matcherName)
        {
        }

        public RegexMatcher(Regex matchRegex, string matcherName = "regex")
        {
            this.matchRegex = matchRegex;
            this.matcherName = matcherName;
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
                    Token = password.Substring(rem.Index, rem.Length)
                });
            }

            return pwMatches;
        }
    }
}
