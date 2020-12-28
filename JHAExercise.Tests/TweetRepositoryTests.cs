using JHAExercise.Models;
using JHAExercise.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static JHAExercise.Models.Extensions;

namespace JHAExercise.Tests
{
    public class TweetRepositoryTests
    {
        private const int TWEET_CREATION_TIMESPAN_MINUTES = 60;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
            .AddDistributedMemoryCache()
            .AddSingleton(SeededTweetCollection())
            .AddTransient<ITweetRepository, TweetRepository>()
            .BuildServiceProvider();
        }

        [TestCase(TWEET_CREATION_TIMESPAN_MINUTES, TestName = "TotalTweetCount_SampleLengthEquals_TWEET_CREATION_TIMESPAN_MINUTES")]
        [Test]
        public void TotalTweetCount_SampleLengthEquals(int expectedResultMinutes)
        {
            var counts = _serviceProvider.GetService<ITweetRepository>().TotalTweetCount();
            Assert.That(counts.SampleLength.TotalMinutes == expectedResultMinutes);
        }

        [TestCase(1, true, TestName = "TotalTweetCount_TotalTweetsReceivedIsGreaterThanOne_True")]
        [TestCase(10, false, TestName = "TotalTweetCount_TotalTweetsReceivedIsGreaterThanTen_False")]
        [Test]
        public void TotalTweetCount_TotalTweetsReceivedGreaterThan(int greaterThanCount, bool expectedResult)
        {
            var counts = _serviceProvider.GetService<ITweetRepository>().TotalTweetCount();
            Assert.That(counts.TotalTweetsReceived > greaterThanCount, Is.EqualTo(expectedResult));
        }

        [TestCase(AverageBy.Hour, 9, TestName = "TotalTweetCount_AveragePerHourEquals_9")]
        [TestCase(AverageBy.Minute, .15, TestName = "TotalTweetCount_AveragePerMinuteEquals__15")]
        [TestCase(AverageBy.Second, .0025, TestName = "TotalTweetCount_AveragePerSecondEquals__0025")]
        [Test]
        public void TotalTweetCount_AverageEquals(AverageBy averageBy, double expectedResult)
        {
            var counts = _serviceProvider.GetService<ITweetRepository>().TotalTweetCount();
            Assert.That(counts.Average(averageBy) == expectedResult);
        }

        private SynchronizedCollection<Tweet> SeededTweetCollection()
        {
            var createdEnd = DateTimeOffset.UtcNow;
            var createdStart = createdEnd.AddMinutes(-TWEET_CREATION_TIMESPAN_MINUTES);
            var tweets = new List<Tweet>();
            tweets.AddRange(new List<Tweet>
            {
                new Tweet { Id = Guid.NewGuid().ToString(), Created = createdStart, Text =  "Hello" },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = createdEnd, Text = "Hello #there", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there friend", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there bob", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there beth", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there jane", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #again", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "again" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there #again", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" }, new Hashtag { Tag = "again" } } } },
                new Tweet { Id = Guid.NewGuid().ToString(), Created = RandomTime(createdStart, createdEnd), Text = "Hello #there #now", Entities = new Entities{ Hashtags  = new List<Hashtag> { new Hashtag { Tag = "there" }, new Hashtag { Tag = "now" } } } }
            });

            return new SynchronizedCollection<Tweet>(new object(), tweets);
        }

        private DateTimeOffset RandomTime(DateTimeOffset start, DateTimeOffset end)
        {
            var random = new Random();
            var maxMinutes = (end - start).Minutes;

            var randomMinutes = random.Next(maxMinutes);
            return start.AddMinutes(-randomMinutes);
        }
    }
}