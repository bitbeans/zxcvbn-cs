using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn.Matcher
{
    public class SequenceMatcher : IMatcher
    {
        // Sequences should not overlap
        string[] Sequences = new string[] { 
            "abcdefghijklmnopqrstuvwxyz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "0123456789"
        };

        const string SequencePattern = "sequence";

        public IEnumerable<Match> MatchPassword(string password)
        {
            // Sequences to check should be the set of sequences and their reverses (i.e. want to match "abcd" and "dcba")
            var seqs = Sequences.Union(Sequences.Select(s => s.StringReverse()));

            var matches = new List<Match>();

            var i = 0;
            while (i < password.Length - 1)
            {
                int j = i + 1;

                // Find a sequence that the current and next characters could be part of 
                var seq = (from s in seqs
                                let ixI = s.IndexOf(password[i])
                                let ixJ = s.IndexOf(password[j])
                                where ixJ == ixI + 1 // i.e. two consecutive letters in password are consecutive in sequence
                                select s).FirstOrDefault();

                // seq will be null when there are no matching sequences
                if (seq != null)
                {
                    var startIndex = seq.IndexOf(password[i]);

                    // Find length of matching sequence (j should be the character after the end of the matching subsequence)
                    for (; j < password.Length && startIndex + j - i < seq.Length && seq[startIndex + j - i] == password[j]; j++) ;

                    var length = j - i;
                    
                    // Only want to consider sequences that are longer than two characters
                    if (length > 2)
                    {
                        matches.Add(new Match() {
                            i = i,
                            j = j - 1,
                            Token = password.Substring(i, j - i),
                            Pattern = SequencePattern,
                            Entropy = CalculateEntropy()
                        });
                    }
                }

                i = j;
            }

            return matches;
        }


        private double CalculateEntropy()
        {
            return 0;
        }
    }
}
