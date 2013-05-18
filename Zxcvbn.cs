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
        private const string BruteforcePattern = "bruteforce";

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

        /// <summary>
        /// Perform the password matching on the given password and user inputs, returing the result structure with information
        /// on the lowest entropy match found.
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="userInputs">Optionally, an enumarable of user data</param>
        /// <returns>Result for lowest entropy match</returns>
        public Result GetPasswordMatches(string password, IEnumerable<string> userInputs = null)
        {
            userInputs = userInputs ?? new string[0];

            IEnumerable<Match> matches = new List<Match>();
            
            var timer = System.Diagnostics.Stopwatch.StartNew();
            
            foreach (var matcher in matcherFactory.CreateMatchers(userInputs))
            {
                matches = matches.Union(matcher.MatchPassword(password));
            }

            var result = FindMinimumEntropyMatch(password, matches);

            timer.Stop();
            result.CalcTime = timer.ElapsedMilliseconds;

            return result;
        }

        /// <summary>
        /// Returns a new result structure initialised with data for the lowest entropy result of all of the matches passed in.
        /// </summary>
        private Result FindMinimumEntropyMatch(string password, IEnumerable<Match> matches)
        {
            var bruteforce_cardinality = PasswordScoring.PasswordCardinality(password);

            // Minimum entropy up to position k in the password
            var minimumEntropyToIndex = new double[password.Length];
            var bestMatchFoIndex = new Match[password.Length];
 
            for (var k = 0; k < password.Length; k++)
            {
                // Start with bruteforce scenario added to previous sequence to beat
                minimumEntropyToIndex[k] = (k == 0 ? 0 : minimumEntropyToIndex[k - 1]) + Math.Log(bruteforce_cardinality, 2);

                // All matches that end at the current character, test to see if the entropy is less
                foreach (var match in matches.Where(m => m.j == k))
                {
                    var candidate_entropy = (match.i <= 0 ? 0 : minimumEntropyToIndex[match.i - 1]) + match.Entropy;
                    if (candidate_entropy < minimumEntropyToIndex[k])
                    {
                        minimumEntropyToIndex[k] = candidate_entropy;
                        bestMatchFoIndex[k] = match;
                    }
                }
            }


            // Walk backwards through lowest entropy matches, to build the best password sequence
            var matchSequence = new List<Match>();
            for (var k = password.Length - 1; k >= 0; k--)
            {
                if (bestMatchFoIndex[k] != null)
                {
                    matchSequence.Add(bestMatchFoIndex[k]);
                    k = bestMatchFoIndex[k].i; // Jump back to start of match
                }
            }
            matchSequence.Reverse();


            // The match sequence might have gaps, fill in with bruteforce matching
            // After this the matches in matchSequence must cover the whole string (i.e. match[k].j == match[k + 1].i - 1)
            if (matchSequence.Count == 0)
            {
                // To make things easy, we'll separate out the case where there are no matches so everything is bruteforced
                matchSequence.Add(new Match()
                {
                    i = 0,
                    j = password.Length,
                    Token = password,
                    Cardinality = bruteforce_cardinality,
                    Pattern = BruteforcePattern,
                    Entropy = Math.Log(Math.Pow(bruteforce_cardinality, password.Length), 2)
                });
            }
            else
            {
                var matchSequenceCopy = new List<Match>();
                for (var k = 0; k < matchSequence.Count; k++)
                {
                    var m1 = matchSequence[k];
                    var m2 = (k < matchSequence.Count - 1 ? matchSequence[k + 1] : new Match() { i = password.Length }); // Next match, or a match past the end of the password

                    matchSequenceCopy.Add(m1);
                    if (m1.j < m2.i - 1)
                    {
                        // Fill in gap
                        var ns = m1.j + 1;
                        var ne = m2.i - 1;
                        matchSequenceCopy.Add(new Match()
                        {
                            i = ns,
                            j = ne,
                            Token = password.Substring(ns, ne - ns + 1),
                            Cardinality = bruteforce_cardinality,
                            Pattern = BruteforcePattern,
                            Entropy = Math.Log(Math.Pow(bruteforce_cardinality, ne - ns + 1), 2)
                        });
                    }
                }

                matchSequence = matchSequenceCopy;
            }


            var minEntropy = (password.Length == 0 ? 0 : minimumEntropyToIndex[password.Length - 1]);
            var crackTime = PasswordScoring.EntropyToCrackTime(minEntropy);

            var result = new Result();
            result.Password = password;
            result.Entropy = Math.Round(minEntropy, 3);
            result.MatchSequence = matchSequence;
            result.CrackTime = Math.Round(crackTime, 3);
            result.CrackTimeDisplay = Utility.DisplayTime(crackTime);
            result.Score = PasswordScoring.CrackTimeToScore(crackTime);

            return result;
        }
        
        /// <summary>
        /// A static function to match a password against the default matchers without having to create
        /// an instance of Zxcvbn yourself, with supplied user data. 
        /// </summary>
        /// <param name="password">the password to test</param>
        /// <param name="userInputs">optionally, the user inputs</param>
        /// <returns></returns>
        public static Result MatchPassword(string password, IEnumerable<string> userInputs = null)
        {
            var zx = new Zxcvbn(new DefaultMatcherFactory());
            return zx.GetPasswordMatches(password, userInputs);
        }

    }
}
