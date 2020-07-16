using System;
using System.Linq;
using System.Windows.Media;

namespace MercurialReport
{
    public class MercurialLogParser
    {
        private readonly string command = "hg log --template {date}%@%{author}%@%{desc}%endline%";
        private readonly string separator = "%@%";
        private readonly string endline = "%endline%";
        public MercurialLogParser(string workingDirectory)
        {
            var log = CommandRunner.ExecuteCommandSync(command, workingDirectory);
            Log = log.Split(endline).Where(s => !string.IsNullOrWhiteSpace(s)).Select(ParseLine).ToArray();
        }

        private LogRecord ParseLine(string line)
        {
            var elements = line.Split(separator);
            if (elements.Length != 3)
                throw new ApplicationException($"line doesn't match template: \"{line}\"");
            var utc = double.Parse(elements[0].Split("-")[0]);
            return new LogRecord
            {
                Date = UnixTimeStampToDateTime(utc),
                Author = elements[1],
                Description = elements[2]
            };

        }
        
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public LogRecord[] Log { get; }


    }
}
