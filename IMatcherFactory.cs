using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// Interface that matcher factories must implement. Matcher factories return a list of the matchers
    /// that will be used to check the password/
    /// </summary>
    public interface IMatcherFactory
    {
        IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs);
    }
}
