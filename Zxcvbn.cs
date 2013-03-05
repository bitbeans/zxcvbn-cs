using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn
{
    public class Zxcvbn
    {
        public static Result MatchPassword(string password)
        {
            return MatchPassword(password, new string[0]);
        }

        public static Result MatchPassword(string password, IEnumerable<string> userInputs)
        {
            return new Result();
        }

    }
}
