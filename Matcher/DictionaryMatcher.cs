using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Zxcvbn.Matcher
{
    public class DictionaryMatcher : IMatcher
    {
        const string DictionaryPattern = "dictionary";

        private string dictionaryName;
        private Lazy<Dictionary<string, int>> rankedDictionary;

        /// <summary>
        /// Creates a new dictionary matcher. wordListPath must be the path (relative or absolute) to a file containing one word per line,
        /// entirely in lowercase, ordered by frequency (decreasing).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="wordListPath"></param>
        public DictionaryMatcher(string name, string wordListPath)
        {
            this.dictionaryName = name;
            rankedDictionary = new Lazy<Dictionary<string, int>>(() => BuildRankedDictionary(wordListPath));
        }

        /// <summary>
        /// Creates a new dictionary matcher from the passed in word list. If there is any frequency order then they should be in
        /// decreasing frequency order.
        /// </summary>
        public DictionaryMatcher(string name, IEnumerable<string> wordList)
        {
            this.dictionaryName = name;

            // Must ensure that the dictionary is using lowercase words only
            rankedDictionary = new Lazy<Dictionary<string, int>>(() => BuildRankedDictionary(wordList.Select(w => w.ToLower())));
        }

        public IEnumerable<Match> MatchPassword(string password)
        {
            var passwordLower = password.ToLower();

            var matches = (from i in Enumerable.Range(0, password.Length)
                          from j in Enumerable.Range(i, password.Length - i)
                          let psub = passwordLower.Substring(i, j - i + 1)
                          where rankedDictionary.Value.ContainsKey(psub)
                          select new DictionaryMatch()
                          {
                              Pattern = DictionaryPattern,
                              i = i,
                              j = j,
                              Token = password.Substring(i, j - i + 1), // Could have different case so pull from password
                              MatchedWord = psub,
                              Rank = rankedDictionary.Value[psub],
                              DictionaryName = dictionaryName,
                              Cardinality = rankedDictionary.Value.Count
                          }).ToList();

            foreach (var match in matches) CalculateEntropyForMatch(match);

            return matches;
        }

        private void CalculateEntropyForMatch(DictionaryMatch match)
        {
            match.BaseEntropy = Math.Log(match.Rank, 2);
            match.UppercaseEntropy = PasswordScoring.CalculateUppercaseEntropy(match.Token);
            
            match.Entropy = match.BaseEntropy + match.UppercaseEntropy;
        }

        

        private Dictionary<string, int> BuildRankedDictionary(string wordListPath)
        {
            return BuildRankedDictionary(File.ReadAllLines(wordListPath));
        }

        private Dictionary<string, int> BuildRankedDictionary(IEnumerable<string> wordList)
        {
            var dict = new Dictionary<string, int>();

            var i = 1;
            foreach (var word in wordList)
            {
                // The word list is assumed to be in increasing frequency order
                dict[word] = i++;
            }

            return dict;
        }
    }

    public class DictionaryMatch : Match
    {
        public string MatchedWord { get; set; }
        public int Rank { get; set; }
        public string DictionaryName { get; set; }

        public double BaseEntropy { get; set; }
        public double UppercaseEntropy { get; set; }
    }
}
