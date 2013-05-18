using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Match repeated characters in the password
    /// </summary>
    public class RepeatMatcher : IMatcher
    {
        const string RepeatPattern = "repeat";

        public IEnumerable<Match> MatchPassword(string password)
        {
            var matches = new List<Match>();

            return password.GroupAdjacent(c => c).Where(g => g.Count() > 1).Select(g => new Match {
                Pattern = RepeatPattern,
                Token = password.Substring(g.StartIndex, g.EndIndex - g.StartIndex + 1),
                i = g.StartIndex,
                j = g.EndIndex,
                Entropy = CalculateEntropy(password.Substring(g.StartIndex, g.EndIndex - g.StartIndex + 1))
            });
        }

        private double CalculateEntropy(string match)
        {
            return Math.Log(PasswordScoring.PasswordCardinality(match) * match.Length, 2);
        }
    }
}
