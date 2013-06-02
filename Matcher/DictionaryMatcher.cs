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
            match.UppercaseEntropy = CalculateUppercaseEntropy(match.Token);
            
            match.Entropy = match.BaseEntropy + match.UppercaseEntropy;
        }

        private double CalculateUppercaseEntropy(string word)
        {
            const string StartUpper = "^[A-Z][^A-Z]+$";
            const string EndUpper = "^[^A-Z]+[A-Z]$";
            const string AllUpper = "^[^a-z]+$";
            const string AllLower = "^[^A-Z]+$";

            if (Regex.IsMatch(word, AllLower)) return 0;

            // If the word is all uppercase add's only one bit of entropy, add only one bit for initial/end caps only
            if (new[] { StartUpper, EndUpper, AllUpper }.Any(re => Regex.IsMatch(word, re))) return 1;

            var lowers = word.Where(c => 'a' <= c && c <= 'z').Count();
            var uppers = word.Where(c => 'A' <= c && c <= 'Z').Count();

            // Calculate numer of ways to capitalise (or inverse if there are fewer lowercase chars) and return lg for entropy
            return Math.Log(Enumerable.Range(0, Math.Min(uppers, lowers)).Sum(i => PasswordScoring.Binomial(uppers + lowers, i)) , 2);
        }

        private Dictionary<string, int> BuildRankedDictionary(string wordListPath)
        {
            var dict = new Dictionary<string, int>();

            var i = 1;
            foreach (var word in File.ReadAllLines(wordListPath))
            {
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
