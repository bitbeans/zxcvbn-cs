using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: These should probably be immutable
// TODO: Describe fields
namespace Zxcvbn
{
    /// <summary>
    /// The results of zxcvbn's password analysis
    /// </summary>
    public class Result
    {
        public double Entropy { get; set; }
        public int CrackTime { get; set; }
        public string CrackTimeDisplay { get; set; }
        public int Score { get; set; }
        public IList<Match> MatchSequence { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// A single match that one of the matchers has made against the password being tested.
    /// </summary>
    public class Match
    {
        public string Pattern { get; set; }
        public string Token { get; set; }
        public double Entropy { get; set; }
        public int Cardinality { get; set; }

        // TODO: Rename, or make internal?
        public int i { get; set; }
        public int j { get; set; }
    }

}
