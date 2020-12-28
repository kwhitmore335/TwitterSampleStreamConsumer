using JHAExercise.Models;
using JHAExercise.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JHAExercise
{
    public class OutputProcessor
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly ILogger<OutputProcessor> _logger;

        public OutputProcessor(ITweetRepository tweetRepository, ILogger<OutputProcessor> logger)
        {
            _tweetRepository = tweetRepository;
            _logger = logger;
        }

        public void ReportOut()
        {
            var tweetCount = _tweetRepository.TotalTweetCount();
            var topEmojis = _tweetRepository.TopEmojis(3);
            var potEmoji = _tweetRepository.PercentageOfTweetsWithEmoji();
            var topHastags = _tweetRepository.TopHashTags(3);
            var potUrl = _tweetRepository.PercentageOfTweetsWithUrl();
            var potPhotoUrl = _tweetRepository.PercentageOfTweetsWithUrl(new List<string> { "pic.twitter.com", "Instagram" });
            var topUrlDomains = _tweetRepository.TopUrlDomains(3);
            _logger.LogInformation($"Total tweets: {tweetCount.TotalTweetsReceived}{Environment.NewLine}" +
                $"Average tweets per hour: {tweetCount.Average(Extensions.AverageBy.Hour):F4}{Environment.NewLine}" +
                $"Average tweets per minute: {tweetCount.Average(Extensions.AverageBy.Minute):F4}{Environment.NewLine}" +
                $"Average tweets per second: {tweetCount.Average(Extensions.AverageBy.Second):F4}{Environment.NewLine}" +
                $"Top Emojis in tweets: {string.Join(",", topEmojis.ToList().OrderByDescending(x => x.Value).Select(x => x.Key))}{Environment.NewLine}" +
                $"Percent of tweets that contain an emoji: {potEmoji:P4}{Environment.NewLine}" +
                $"Top hashtags: {string.Join(",", topHastags.ToList().OrderByDescending(x => x.Value).Select(x => x.Key))}{Environment.NewLine}" +
                $"Percent of tweets that contain a url: {potUrl:P4}{Environment.NewLine}" +
                $"Percent of tweets that contain a photo url: {potPhotoUrl:P4}{Environment.NewLine}" +
                $"Top domains of urls in tweets: {string.Join(",", topUrlDomains.ToList().OrderByDescending(x => x.Value).Select(x => x.Key))}{Environment.NewLine}", new object[0]);
        }
    }
}
