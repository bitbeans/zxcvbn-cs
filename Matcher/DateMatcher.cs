using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zxcvbn.Matcher
{
    // TODO: This whole class is a bit messy but works, could do with a touching up
    public class DateMatcher : IMatcher
    {
        const string DatePattern = "date";


        // The two regexes for matching dates with slashes is lifted directly from zxcvbn (matching.coffee about :400)
        const string DateWithSlashesSuffixPattern = @"  ( \d{1,2} )                         # day or month
  ( \s | - | / | \\ | _ | \. )        # separator
  ( \d{1,2} )                         # month or day
  \2                                  # same separator
  ( 19\d{2} | 200\d | 201\d | \d{2} ) # year";

        const string DateWithSlashesPrefixPattern = @"  ( 19\d{2} | 200\d | 201\d | \d{2} ) # year
  ( \s | - | / | \\ | _ | \. )        # separator
  ( \d{1,2} )                         # day or month
  \2                                  # same separator
  ( \d{1,2} )                         # month or day";

        public IEnumerable<Match> MatchPassword(string password)
        {           
            var matches = new List<Match>();

            var possibleDates = Regex.Matches(password, "\\d{4,8}"); // Slashless dates
            foreach (System.Text.RegularExpressions.Match dateMatch in possibleDates)
            {
                if (IsDate(dateMatch.Value)) matches.Add(new Match()
                {
                    Pattern = DatePattern,
                    i = dateMatch.Index,
                    j = dateMatch.Index + dateMatch.Length - 1,
                    Token = dateMatch.Value
                });
            }

            var slashDatesSuffix = Regex.Matches(password, DateWithSlashesSuffixPattern, RegexOptions.IgnorePatternWhitespace);
            foreach (System.Text.RegularExpressions.Match dateMatch in slashDatesSuffix)
            {
                var year = dateMatch.Groups[4].Value.ToInt();
                var month = dateMatch.Groups[3].Value.ToInt(); // or day
                var day = dateMatch.Groups[1].Value.ToInt(); // or month
                if (IsDateInRange(year, month, day) || IsDateInRange(year, day, month)) matches.Add(new Match()
                {
                    Pattern = DatePattern,
                    i = dateMatch.Index,
                    j = dateMatch.Index + dateMatch.Length - 1,
                    Token = dateMatch.Value
                });
            }

            var slashDatesPrefix = Regex.Matches(password, DateWithSlashesPrefixPattern, RegexOptions.IgnorePatternWhitespace);
            foreach (System.Text.RegularExpressions.Match dateMatch in slashDatesPrefix)
            {
                var year = dateMatch.Groups[1].Value.ToInt();
                var month = dateMatch.Groups[3].Value.ToInt(); // or day
                var day = dateMatch.Groups[4].Value.ToInt(); // or month
                if (IsDateInRange(year, month, day) || IsDateInRange(year, day, month)) matches.Add(new Match()
                {
                    Pattern = DatePattern,
                    i = dateMatch.Index,
                    j = dateMatch.Index + dateMatch.Length - 1,
                    Token = dateMatch.Value
                });
            }

            return matches;
        }

        /// <summary>
        /// Determine whether a string resembles a date (year first or year last)
        /// </summary>
        private Boolean IsDate(string match)
        {
            bool isValid = false;
            
            // Try year length depending on match length. Length six should try both two and four digits
            
            if (match.Length <= 6)
            {
                // Try a two digit year, suffix and prefix
                isValid |= IsDateWithYearType(match, true, 2);
                isValid |= IsDateWithYearType(match, false, 2);
            }
            if (match.Length >= 6)
            {
                // Try a four digit year, suffix and prefix
                isValid |= IsDateWithYearType(match, true, 4);
                isValid |= IsDateWithYearType(match, false, 4);
            }

            return isValid;
        }

        private Boolean IsDateWithYearType(string match, bool suffix, int yearLen)
        {
            int year = 0;
            if (suffix) match.IntParseSubstring(match.Length - yearLen, yearLen, out year);
            else match.IntParseSubstring(0, yearLen, out year);

            if (suffix) return IsYearInRange(year) && IsDayMonthString(match.Substring(0, match.Length - yearLen));
            else return IsYearInRange(year) && IsDayMonthString(match.Substring(yearLen, match.Length - yearLen));
        }

        /// <summary>
        /// Determines whether a substring of a date string resembles a day and month (day-month or month-day)
        /// </summary>
        private Boolean IsDayMonthString(string match)
        {
            int p1 = 0, p2 = 0;
            
            // Parse the day/month string into two parts
            if (match.Length == 2)
            {
                // e.g. 1 2 [1234]
                match.IntParseSubstring(0, 1, out p1);
                match.IntParseSubstring(1, 1, out p2);
            }
            else if (match.Length == 3)
            {
                // e.g. 1 12 [1234] or 12 1 [1234]

                match.IntParseSubstring(0, 1, out p1);
                match.IntParseSubstring(1, 2, out p2);

                // This one is a little different in that there's two ways to parse it so go one way first
                if (IsMonthDayInRange(p1, p2) || IsMonthDayInRange(p2, p1)) return true;

                match.IntParseSubstring(0, 2, out p1);
                match.IntParseSubstring(2, 1, out p2);
            }
            else if (match.Length == 4)
            {
                // e.g. 14 11 [1234]

                match.IntParseSubstring(0, 2, out p1);
                match.IntParseSubstring(2, 2, out p2);
            }

            // Check them both ways around to see if a valid day/month pair
            return IsMonthDayInRange(p1, p2) || IsMonthDayInRange(p2, p1);
        }

        private Boolean IsDateInRange(int year, int month, int day)
        {
            return IsYearInRange(year) && IsMonthDayInRange(month, day);
        }

        // Two-digit years are allowed, otherwise in 1900-2019
        private Boolean IsYearInRange(int year)
        {
            return (1900 <= year && year <= 2019) || (0 < year && year <= 99);
        }

        // Assume all months have 31 days, we only care that things look like dates not that they're completely valid
        private Boolean IsMonthDayInRange(int month, int day)
        {
            return 1 <= month && month <= 12 && 1 <= day && day <= 31;
        }
    }
}
