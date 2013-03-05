using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn.Matcher
{
    class NullMatcher : IMatcher
    {
        public IEnumerable<Match> MatchPassword(string password)
        {
            return new Match[0];
        }
    }
}
