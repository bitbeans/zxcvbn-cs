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
                new SequenceMatcher()
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
