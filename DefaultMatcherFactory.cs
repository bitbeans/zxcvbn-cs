using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// This matcher factory will use all of the default password matchers.
    /// </summary>
    class DefaultMatcherFactory : IMatcherFactory
    {
        IMatcher[] matchers;

        public DefaultMatcherFactory()
        {
            matchers = new IMatcher[] {
                new RepeatMatcher(),
                new SequenceMatcher(),
                new RegexMatcher("\\d{3,}", 10, "digits"),
                new RegexMatcher("19\\d\\d|200\\d|201\\d", 119, "year"),
                new DateMatcher(),
                new SpatialMatcher()
            };
        }

        public DefaultMatcherFactory(IMatcher[] matchers)
        {
            this.matchers = matchers;
        }

        public IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            return matchers;
        }
    }
}
