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
        List<IMatcher> matchers;

        public DefaultMatcherFactory()
        {
            var dictionaryMatchers = new List<DictionaryMatcher>() {
                new DictionaryMatcher("passwords", "passwords.lst"),
                new DictionaryMatcher("english", "english.lst"),
                new DictionaryMatcher("male_names", "male_names.lst"),
                new DictionaryMatcher("female_names", "female_names.lst"),
                new DictionaryMatcher("surnames", "surnames.lst"),
            };

            matchers = new List<IMatcher> {
                new RepeatMatcher(),
                new SequenceMatcher(),
                new RegexMatcher("\\d{3,}", 10, true, "digits"),
                new RegexMatcher("19\\d\\d|200\\d|201\\d", 119, false, "year"),
                new DateMatcher(),
                new SpatialMatcher()
            };

            matchers.AddRange(dictionaryMatchers);
            matchers.Add(new L33tMatcher(dictionaryMatchers));
        }

        public DefaultMatcherFactory(List<IMatcher> matchers)
        {
            this.matchers = matchers;
        }

        public IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            var userInputDict = new DictionaryMatcher("user_inputs", userInputs);
            var leetUser = new L33tMatcher(userInputDict);

            return matchers.Union(new List<IMatcher> { userInputDict, leetUser });
        }
    }
}
