using System;

namespace JHAExercise.Models
{
    public class TotalTweetCount
    {
        public int TotalTweetsReceived { get; set; }
        public TimeSpan SampleLength { get; set; }        
    }

    public static partial class Extensions
    {
        public static double Average(this TotalTweetCount ttc, AverageBy averageBy)
        {
            double result = 0;
            switch (averageBy)
            {
                case AverageBy.Second:
                    result = ttc.TotalTweetsReceived / ttc.SampleLength.TotalSeconds;
                    break;
                case AverageBy.Minute:
                    result = ttc.TotalTweetsReceived / ttc.SampleLength.TotalMinutes;
                    break;
                case AverageBy.Hour:
                    result = ttc.TotalTweetsReceived / ttc.SampleLength.TotalHours;
                    break;
            }
            return result;
        }

        public enum AverageBy
        { 
            Second,
            Minute,
            Hour
        }
    }
    
}

