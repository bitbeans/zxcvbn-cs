using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn
{
    static class PasswordScoring
    {
        /// <summary>
        /// Calculate the cardinality of the minimal character sets necessary to brute force the password (roughly)
        /// (e.g. lowercase = 26, numbers = 10, lowercase + numbers = 36)
        /// </summary>
        public static int PasswordCardinality(string password)
        {
            var cl = 0;
            
            if (password.Any(c => 'a' <= c && c <= 'z')) cl += 26; // Lowercase
            if (password.Any(c => 'A' <= c && c <= 'Z')) cl += 26; // Uppercase
            if (password.Any(c => '0' <= c && c <= '9')) cl += 10; // Numbers
            if (password.Any(c => c <= '/' || (':' <= c && c <= '@') || ('[' <= c && c <= '`') || ('{' <= c && c <= 0x7F))) cl += 33; // Symbols
            if (password.Any(c => c > 0x7F)) cl += 100; // 'Unicode' (why 100?)

            return cl;
        }

        /// <summary>
        /// Calculate a rough estimate of crack time for entropy, see zxcbn scoring.coffee for more information on the model used
        /// </summary>
        /// <param name="entropy"></param>
        /// <returns></returns>
        public static double EntropyToCrackTime(double entropy)
        {
            const double SingleGuess = 0.01;
            const double NumAttackers = 100;
            const double SecondsPerGuess = SingleGuess / NumAttackers;

            return 0.5 * Math.Pow(2, entropy) * SecondsPerGuess;
        }

        /// <summary>
        /// Return a score for password strength from the crack time. Scores are 0..4, 0 being minimum and 4 maximum strength.
        /// </summary>
        /// <param name="crackTime"></param>
        /// <returns>Password strength. 0 to 4, 0 is minimum</returns>
        public static int CrackTimeToScore(double crackTimeSeconds)
        {
            if (crackTimeSeconds < Math.Pow(10, 2)) return 0;
            else if (crackTimeSeconds < Math.Pow(10, 4)) return 1;
            else if (crackTimeSeconds < Math.Pow(10, 6)) return 2;
            else if (crackTimeSeconds < Math.Pow(10, 8)) return 3;
            else return 4;
        }


    }
}
