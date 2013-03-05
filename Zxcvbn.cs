using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// Zxcvbn is used to estimate the strength of passwords. 
    /// 
    /// This implementation is a port of the Zxcvbn JavaScript library by Dan Wheeler:
    /// https://github.com/lowe/zxcvbn
    /// </summary>
    public class Zxcvbn
    {
        private IMatcherFactory matcherFactory;

        /// <summary>
        /// Create an instance of Zxcvbnm that will use the given matcher factory to create matchers to use
        /// to find password weakness.
        /// </summary>
        /// <param name="matcherFactory"></param>
        public Zxcvbn(IMatcherFactory matcherFactory)
        {
            this.matcherFactory = matcherFactory;
        }

        public Result GetPasswordMatches(string password, IEnumerable<string> userInputs)
        {
            userInputs = userInputs ?? new string[0];

            IEnumerable<Match> matches = new List<Match>();

            foreach (var matcher in matcherFactory.CreateMatchers(userInputs))
            {
                matches = matches.Union(matcher.MatchPassword(password));
            }

            return new Result();
        }


        /// <summary>
        /// A static function to match a password against the default matchers without having to create
        /// an instance of Zxcvbn yourself
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Result MatchPassword(string password)
        {
            return MatchPassword(password, new string[0]);
        }
        
        /// <summary>
        /// A static function to match a password against the default matchers without having to create
        /// an instance of Zxcvbn yourself, with supplied user data. 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="userInputs"></param>
        /// <returns></returns>
        public static Result MatchPassword(string password, IEnumerable<string> userInputs)
        {
            var zx = new Zxcvbn(new DefaultMatcherFactory());
            return zx.GetPasswordMatches(password, userInputs);
        }

    }
}
