using System;

namespace Code.Common
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan timeSpan, int number)
        {
            return TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds * number);
        } 
        
        public static TimeSpan Multiply(this TimeSpan timeSpan, double number)
        {
            return TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds * number);
        } 
    }
}