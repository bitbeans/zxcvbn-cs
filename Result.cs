using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: These should probably be immutable
namespace Zxcvbn
{
    public class Result
    {
        public double Entropy { get; set; }
        public int CrackTime { get; set; }
        public string CrackTimeDisplay { get; set; }
        public int Score { get; set; }
        public IList<Match> MatchSequence { get; set; }
        public string Password { get; set; }
    }

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
