using JHAExercise.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using JHAExercise.Constants;

namespace JHAExercise.Repositories
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IDistributedCache _cache;
        private readonly SynchronizedCollection<Tweet> _tweetStore;

        public TweetRepository(IDistributedCache cache, SynchronizedCollection<Tweet> tweetStore)
        {
            _cache = cache;
            _tweetStore = tweetStore;
        }

        public void Create(Tweet tweet)
        {
            lock (_tweetStore.SyncRoot)
            {
                _tweetStore.Add(tweet);
            }                
        }

        public TotalTweetCount TotalTweetCount()
        {
            TotalTweetCount result = null;

            lock (_tweetStore.SyncRoot)
            {
                var createds = _tweetStore.Select(t => t.Created);
                result = new TotalTweetCount
                {
                    TotalTweetsReceived = _tweetStore?.Count ?? 0,
                    SampleLength = createds.Max().Subtract(createds.Min()),

                };
            }

            return result;
        }

        public Dictionary<string, int> TopEmojis(int top)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            lock (_tweetStore.SyncRoot)
            {
               result = _tweetStore.Where(t => t.Entities?.Emojis != null)
                .SelectMany(t => t.Entities.Emojis)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count()).Take(top)
                .ToDictionary(x => x.Key, x => x.Count());
            }

            return result;
        }

        public Dictionary<string, int> TopHashTags(int top)
        {
            Dictionary<string, int> result;

            lock (_tweetStore.SyncRoot)
            {
                result = _tweetStore.Where(t => t.Entities?.Hashtags != null)
                .SelectMany(t => t.Entities.Hashtags)
                .GroupBy(x => x.Tag)
                .OrderByDescending(x => x.Count()).Take(top)
                .ToDictionary(x => x.Key, x => x.Count());
            }

            return result;
        }

        public double PercentageOfTweetsWithEmoji()
        {
            double result;

            lock (_tweetStore.SyncRoot)
            {
                result = (double)_tweetStore.Where(t => t.Entities?.Emojis?.Any() ?? false).Count() / (double)_tweetStore.Count();
            }

            return result;
        }

        public double PercentageOfTweetsWithUrl(IEnumerable<string> containingText = null)
        {
            double result;

            Func<Tweet, bool> whereClause = (t => t.Entities?.Urls?.Any() ?? false);
            if (containingText?.Count() > 0)
            {
                containingText = containingText.Select(x => x?.ToLower());
                whereClause = (t => t.Entities?.Urls?
                    .Any(u => u.Url != null && containingText.Any(ct => u.Url.ToLower().Contains(ct)))
                    ?? false);
            }

            lock (_tweetStore.SyncRoot)
            {
                result = (double)_tweetStore.Where(whereClause).Count() / (double)_tweetStore.Count();
            }

            return result;
        }

        public Dictionary<string, int> TopUrlDomains(int top)
        {
            Dictionary<string, int> result;

            lock (_tweetStore.SyncRoot)
            {
                result = _tweetStore.Where(t => t.Entities?.Urls != null)
                .SelectMany(t => t.Entities.Urls)
                .GroupBy(x => new Uri(x.Url).Host)
                .OrderByDescending(x => x.Count()).Take(top)
                .ToDictionary(x => x.Key, x => x.Count());
            }

            return result;
        }

        public async Task<IEnumerable<EmojiData>> GetEmojiLookup()
        {
            IEnumerable<EmojiData> emojis;

            var emojisJson = await _cache.GetStringAsync(CacheKeys.EMOJIS);
            if (emojisJson != null)
            {
                emojis = JsonConvert.DeserializeObject<IEnumerable<EmojiData>>(emojisJson);
            }
            else
            {
                using var file = File.OpenText(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).FullName, "emoji.json"));
                var serializer = new JsonSerializer();
                emojis = (IEnumerable<EmojiData>)serializer.Deserialize(file, typeof(IEnumerable<EmojiData>));
                await _cache.SetStringAsync(CacheKeys.EMOJIS, JsonConvert.SerializeObject(emojis),
                    new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            }

            return emojis;
        }
    }
}
