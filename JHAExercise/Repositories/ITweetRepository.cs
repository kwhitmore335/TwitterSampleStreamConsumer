using JHAExercise.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JHAExercise.Repositories
{
    public interface ITweetRepository
    {
        void Create(Tweet tweet);
        TotalTweetCount TotalTweetCount();
        Dictionary<string, int> TopEmojis(int top);
        Dictionary<string, int> TopHashTags(int top);
        double PercentageOfTweetsWithEmoji();
        double PercentageOfTweetsWithUrl(IEnumerable<string> containingText = null);
        Dictionary<string, int> TopUrlDomains(int top);
        Task<IEnumerable<EmojiData>> GetEmojiLookup();
    }
}
