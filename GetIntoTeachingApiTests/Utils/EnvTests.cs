﻿using System;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    // We're changing the environment in these tests.
    [Collection(nameof(NotThreadSafeResourceCollection))]
    public class EnvTests : IDisposable
    {
        private readonly string _previousEnvironment;
        private readonly IEnv _env;

        public EnvTests()
        {
            _env = new Env();
            _previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _previousEnvironment);
        }

        [Theory]
        [InlineData("0", true)]
        [InlineData("1", false)]
        [InlineData("10", false)]
        public void ExportHangfireToPrometheus_TrueOnlyForFirstInstance(string instance, bool expected)
        {
            Environment.SetEnvironmentVariable("CF_INSTANCE_INDEX", instance);

            _env.ExportHangireToPrometheus.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", true)]
        [InlineData("Staging", false)]
        [InlineData("Production", false)]
        [InlineData(null, false)]
        public void IsDevelopment_TrueOnlyForDevelopment(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsDevelopment.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", false)]
        [InlineData("Production", true)]
        [InlineData(null, false)]
        public void IsProduction_TrueOnlyForProduction(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsProduction.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", true)]
        [InlineData("Production", false)]
        [InlineData(null, false)]
        public void IsStaging_TrueOnlyForStaging(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsStaging.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("Development", false)]
        [InlineData("Staging", false)]
        [InlineData("Production", false)]
        public void IsTest_TrueOnlyForNull(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsTest.Should().Be(expected);
        }

        [Fact]
        public void DatabaseInstanceName_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("DATABASE_INSTANCE_NAME");
            Environment.SetEnvironmentVariable("DATABASE_INSTANCE_NAME", "db-instance-name");

            _env.DatabaseInstanceName.Should().Be("db-instance-name");

            Environment.SetEnvironmentVariable("DATABASE_INSTANCE_NAME", previous);
        }

        [Fact]
        public void HangfireInstanceName_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("HANGFIRE_INSTANCE_NAME");
            Environment.SetEnvironmentVariable("HANGFIRE_INSTANCE_NAME", "hangfire-instance-name");

            _env.HangfireInstanceName.Should().Be("hangfire-instance-name");

            Environment.SetEnvironmentVariable("HANGFIRE_INSTANCE_NAME", previous);
        }

        [Fact]
        public void TotpSecretKey_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
            Environment.SetEnvironmentVariable("TOTP_SECRET_KEY", "totp-secret-key");

            _env.TotpSecretKey.Should().Be("totp-secret-key");

            Environment.SetEnvironmentVariable("TOTP_SECRET_KEY", previous);
        }

        [Fact]
        public void GitCommitSha_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA");
            Environment.SetEnvironmentVariable("GIT_COMMIT_SHA", "abc123");

            _env.GitCommitSha.Should().Be("abc123");

            Environment.SetEnvironmentVariable("GIT_COMMIT_SHA", previous);
        }

        [Fact]
        public void VcapServices_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            Environment.SetEnvironmentVariable("VCAP_SERVICES", "vcap-services");

            _env.VcapServices.Should().Be("vcap-services");

            Environment.SetEnvironmentVariable("VCAP_SERVICES", previous);
        }

        [Fact]
        public void CrmServiceUrl_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", "crm-service-url");

            _env.CrmServiceUrl.Should().Be("crm-service-url");

            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", previous);
        }

        [Fact]
        public void CrmClientId_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", "crm-client-id");

            _env.CrmClientId.Should().Be("crm-client-id");

            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", previous);
        }

        [Fact]
        public void CrmClientSecret_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", "crm-client-secret");

            _env.CrmClientSecret.Should().Be("crm-client-secret");

            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", previous);
        }

        [Fact]
        public void NotifyApiKey_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
            Environment.SetEnvironmentVariable("NOTIFY_API_KEY", "notify-api-key");

            _env.NotifyApiKey.Should().Be("notify-api-key");

            Environment.SetEnvironmentVariable("NOTIFY_API_KEY", previous);
        }

        [Fact]
        public void GoogleApiKey_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            Environment.SetEnvironmentVariable("GOOGLE_API_KEY", "google-api-key");

            _env.GoogleApiKey.Should().Be("google-api-key");

            Environment.SetEnvironmentVariable("GOOGLE_API_KEY", previous);
        }

        [Theory]
        [InlineData("0", true)]
        [InlineData(null, true)]
        [InlineData("invalid", true)]
        [InlineData("1", false)]
        [InlineData("-1", false)]
        public void IsMasterInstance_ReturnsCorrectly(string index, bool expectedOutcome)
        {
            var previous = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");
            Environment.SetEnvironmentVariable("CF_INSTANCE_INDEX", index);

            _env.IsMasterInstance.Should().Be(expectedOutcome);

            Environment.SetEnvironmentVariable("CF_INSTANCE_INDEX", previous);
        }

        [Theory]
        [InlineData(null, "Test")]
        [InlineData("Development", "Development")]
        [InlineData("Staging", "Staging")]
        [InlineData("Production", "Production")]
        public void EnvironmentName_ReturnsCorrectly(string environment, string expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.EnvironmentName.Should().Be(expected);
        }

        [Fact]
        public void Get_WithVariable_ReturnsMatchingEnvironmentVariable()
        {
            var previous = Environment.GetEnvironmentVariable("AN_ENV_VAR");
            Environment.SetEnvironmentVariable("AN_ENV_VAR", "test");

            _env.Get("AN_ENV_VAR").Should().Be("test");

            Environment.SetEnvironmentVariable("AN_ENV_VAR", previous);
        }

        [Fact]
        public void Get_WhenVariableDoesNotExist_ReturnsNull()
        {
            _env.Get("NON_EXISTANT").Should().BeNull();
        }
    }
}
