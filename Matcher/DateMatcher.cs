using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zxcvbn.Matcher
{
    public class DateMatcher : IMatcher
    {

        public IEnumerable<Match> MatchPassword(string password)
        {
            var possibleDates = Regex.Matches(password, "\\d{4,8}");
            
            var matches = new List<Match>();

            return matches;
        }

        public Boolean IsDate(string match)
        {
            return false;
        }
    }
}
