using JHAExercise.Clients;
using JHAExercise.Models;
using JHAExercise.Repositories;
using NeoSmart.Unicode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JHAExercise
{
    public class InputProcessor
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly ITwitterRestClient _twitterAPIClient;

        public InputProcessor(ITweetRepository tweetRepository, ITwitterRestClient twitterAPIClient)
        {
            _tweetRepository = tweetRepository;
            _twitterAPIClient = twitterAPIClient;
        }

        public async Task<System.Net.HttpStatusCode> ConsumeTweets()
        {
            var emojis = (await _tweetRepository.GetEmojiLookup()).OrderByDescending(e => e.UnicodeCodepoint.Length);
            var streamRequest = new RestRequest($"{_twitterAPIClient.HostUrl}/2/tweets/sample/stream?tweet.fields=created_at,entities", Method.GET)
            {
                ResponseWriter = (stream) =>
                {
                    using var reader = new StreamReader(stream);
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        _ = Task.Run(() =>
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                var tweet = JsonConvert.DeserializeObject<Tweet>(JObject.Parse(line).SelectToken("data").ToString());
                                var tweetEmojis = EmojisFromString(tweet.Text, emojis);

                                if (tweetEmojis != null)
                                {
                                    tweet.Entities ??= new Entities();
                                    tweet.Entities.Emojis = tweetEmojis;
                                }

                                _tweetRepository.Create(tweet);
                            }
                        });
                    }
                }
            };

            var response = await _twitterAPIClient.ExecuteRequest(streamRequest);
            return response.StatusCode;
        }

        private IEnumerable<string> EmojisFromString(string text, IOrderedEnumerable<EmojiData> emojiData)
        {
            var codepointString = string.Join('-', text.Codepoints().Select(x => $"{x.Value:X}"));

            var tweetEmojis = new List<string>();
            foreach (var e in emojiData)
            {
                if (codepointString.Contains(e.UnicodeCodepoint))
                {
                    tweetEmojis.Add(e.Name);
                    codepointString = codepointString.Replace(e.UnicodeCodepoint, string.Empty);
                }

                if (codepointString.Length == 0)
                { break; }
            }

            return tweetEmojis.Count > 0 ? tweetEmojis : null;
        }
    }
}
