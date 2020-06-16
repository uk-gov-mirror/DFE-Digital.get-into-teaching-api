﻿using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockCrm = new Mock<ICrmService>();
            _controller = new CandidatesController(_mockTokenService.Object, _mockNotifyService.Object, _mockCrm.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void CreateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new ExistingCandidateRequest { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.CreateAccessToken(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void CreateAccessToken_ValidRequest_SendsPINCodeEmail()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var candidate = new Candidate { Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            _mockTokenService.Setup(mock => mock.GenerateToken(request)).Returns("123456");
            _mockCrm.Setup(mock => mock.GetCandidate(request)).Returns(candidate);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NoContentResult>();
            _mockNotifyService.Verify(
                mock => mock.SendEmailAsync(
                    "email@address.com",
                    NotifyService.NewPinCodeEmailTemplateId,
                    It.Is<Dictionary<string, dynamic>>(personalisation => personalisation["pin_code"] as string == "123456")
                )
            );
        }

        [Fact]
        public void CreateAccessToken_MismatchedCandidate_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(mock => mock.GetCandidate(request)).Returns<Candidate>(null);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();
            _mockNotifyService.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()),
                Times.Never()
            );
        }
    }
}
