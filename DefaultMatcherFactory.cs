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
        public IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            return new IMatcher[] {
                                      new NullMatcher()
                                  };
        }
    }
}
