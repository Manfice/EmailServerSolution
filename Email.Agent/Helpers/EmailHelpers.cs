using System;

namespace Email.Agent.Helpers
{
    public class EmailHelpers : IDisposable
    {
        enum MonthName
        {
            Jan = 1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
        }

        public string DateForEmailFiler(DateTime date)
        {
            return $"{date.Day.ToString().PadLeft(2, '0')}-{(MonthName)date.Month}-{date.Year}";

        }

        public void Dispose()
        {
        }
    }
}