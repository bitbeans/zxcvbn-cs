using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn.Matcher
{
    public interface IMatcher
    {
        IEnumerable<Match> MatchPassword(string password);
    }
}
