using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cron.net
{
    public class CronCommandLine
    {
        public IReadOnlyList<int> Minutes { get; }
        public IReadOnlyList<int> Hours { get; }
        public IReadOnlyList<int> DaysOfMonth { get; }
        public IReadOnlyList<int> Months { get; }
        public IReadOnlyList<int> DaysOfWeek { get; }
        public string Command { get; }

        public CronCommandLine(string line)
        {
            var args = Regex.Split(line, "[\\s\\t]+");
            Command = string.Join(" ", args.Skip(5));
            Minutes = ParseIntString(args[0], 60);
            Hours = ParseIntString(args[1], 24);
            DaysOfMonth = ParseIntString(args[2], 31);
            Months = ParseIntString(args[3], 12);
            DaysOfWeek = ParseIntString(args[4], 7);
        }

        private static IReadOnlyList<int> ParseIntString(string numberString, int max)
        {
            var array = Enumerable.Range(0, max);
            if (numberString != "*")
            {
                var dims = numberString.Split('/');
                if (dims.Length > 2)
                {
                    throw new InvalidCronLineException();
                }
                var del = dims.Length > 1 ? int.Parse(dims[1]) : 1;
                if (dims[0] == "*")
                {
                    array = array.Where(i => i % del == 0);
                }
                else if (dims[0].Contains("-"))
                {
                    var range = dims[0].Split('-').Select(int.Parse).ToArray();
                    var nums = Enumerable.Range(range[0], range[1]);
                    array = nums.Where(i => i%del == range[0]);
                }
                else
                {
                    var nums = dims[0].Split(',').Select(int.Parse);
                    array = nums.Where(i => i%del == 0);
                }
            }
            return array.ToList().AsReadOnly();
        }

        public bool CheckDateTime(DateTime? dateTime = null)
        {
            var time = dateTime ?? DateTime.Now;
            return Minutes.Contains(time.Minute) &&
                   Hours.Contains(time.Hour) &&
                   Months.Contains(time.Month - 1) &&
                   (DaysOfMonth.Contains(time.Day - 1) || DaysOfWeek.Contains((int)time.DayOfWeek - 1));
        }
    }

    internal class InvalidCronLineException : Exception
    {
    }

}