﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class ApiClientHandlerTests
    {
        private readonly ApiClientHandler _handler;
        private readonly Mock<ILogger<ApiClientHandler>> _mockLogger;
        private readonly Mock<IClientManager> _mockClientManager;
        private readonly Mock<IEnv> _mockEnv;

        public ApiClientHandlerTests()
        {
            _mockClientManager = new Mock<IClientManager>();
            _mockEnv = new Mock<IEnv>();

            var mockOptionsMonitor = new Mock<IOptionsMonitor<ApiClientSchemaOptions>>();
            mockOptionsMonitor.Setup(m => m.Get("ApiClientHandler")).Returns(new ApiClientSchemaOptions());

            _mockLogger = new Mock<ILogger<ApiClientHandler>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _handler = new ApiClientHandler(_mockEnv.Object, _mockClientManager.Object, mockOptionsMonitor.Object,
                mockLoggerFactory.Object, new Mock<UrlEncoder>().Object, new Mock<ISystemClock>().Object);
        }

        [Theory]
        [InlineData("Bearer api_key", true)]
        [InlineData("api_key", true)]
        [InlineData("Bearer incorect_api_key", false)]
        [InlineData("Bearer api_key ", false)]
        [InlineData("Bearer ", false)]
        [InlineData("api key", false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public async void InitializeAsync_WithApiClient_AuthenticatesCorrectly(string authHeaderValue, bool expected)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Service", ApiKey = "api_key", ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().Be(expected);

            if (result.Succeeded)
            {
                result.Principal.HasClaim("token", "api_key").Should().BeTrue();
                result.Principal.HasClaim(ClaimTypes.Role, "Service").Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("Bearer api_key", true)]
        [InlineData("api_key", true)]
        [InlineData("Bearer incorrect_api_key", false)]
        [InlineData("Bearer api_key ", false)]
        [InlineData("Bearer ", false)]
        [InlineData("api key", false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public async void InitializeAsync_WithSharedSecret_AuthenticatesCorrectly(string authHeaderValue, bool expected)
        {
            _mockEnv.Setup(m => m.SharedSecret).Returns("api_key");
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().Be(expected);

            if (result.Succeeded)
            {
                result.Principal.HasClaim("token", "api_key").Should().BeTrue();
                result.Principal.HasClaim(ClaimTypes.Role, "Admin").Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void InitializeAsync_WhenClientApiKeyIsNotSet_ReturnsNoResult(string apiKey)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Admin", ApiKey = apiKey, ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
        }

        [Theory]
        [InlineData("Bearer ", "")]
        [InlineData("Bearer ", null)]
        [InlineData("Bearer ", " ")]
        [InlineData("Bearer  ", " ")]
        [InlineData("Bearer", "")]
        [InlineData("Bearer", null)]
        [InlineData("Bearer", " ")]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData(" ", " ")]
        [InlineData(" ", "")]
        [InlineData(" ", null)]
        public async void InitializeAsync_EmptyOrNullHeaderAndApiKey_ReturnsUnauthorized(string authHeaderValue, string apiKey)
        {
            _mockEnv.Setup(m => m.SharedSecret).Returns(apiKey);

            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async void InitializeAsync_NoAuthorizationHeader_ReturnsUnauthorized()
        {
            var context = new DefaultHttpContext();
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async void InitializeAsync_IncorrectAuthorizationHeader_LogsWarning()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "incorrect_admin_secret");
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            await _handler.AuthenticateAsync();

            _mockLogger.VerifyWarningWasCalled("ApiClientHandler - API key is not valid");
        }
    }
}