﻿using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class AppSettingsTests
    {
        private readonly AppSettings _settings;
        private readonly Mock<IDatabase> _database;

        public AppSettingsTests()
        {
            _database = new Mock<IDatabase>();
            var redis = new Mock<IRedisService>();
            redis.Setup(m => m.Database).Returns(_database.Object);

            _settings = new AppSettings(redis.Object);
        }

        [Fact]
        public void CrmIntegrationPausedUntil_SetAndGetWithDate_WorkCorrectly()
        {
            var date = DateTime.UtcNow;
            var dateStr = date.ToString("O");
            _database.Setup(m => m.StringSet("app_settings.crm_offline_until", dateStr, null, When.Always, CommandFlags.None));
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.CrmIntegrationPausedUntil = date;
            _settings.CrmIntegrationPausedUntil.Should().Be(date);
        }

        [Fact]
        public void CrmIntegrationPausedUntil_SetAndGetWithNull_WorkCorrectly()
        {
            _database.Setup(m => m.StringSet("app_settings.crm_offline_until", null as string, null, When.Always, CommandFlags.None));
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(null as string);

            _settings.CrmIntegrationPausedUntil = null;
            _settings.CrmIntegrationPausedUntil.Should().BeNull();
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenCrmOfflineUntilIsNull_ReturnsFalse()
        {
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns<string>(null);

            _settings.IsCrmIntegrationPaused.Should().BeFalse();
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenCrmOfflineUntilIsAFutureDate_ReturnsTrue()
        {
            var dateStr = DateTime.UtcNow.AddDays(1).ToString("O");
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.IsCrmIntegrationPaused.Should().BeTrue();
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenCrmOfflineUntilIsAPastDate_ReturnsFalse()
        {
            var dateStr = DateTime.UtcNow.AddDays(-1).ToString("O");
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.IsCrmIntegrationPaused.Should().BeFalse();
        }
    }
}