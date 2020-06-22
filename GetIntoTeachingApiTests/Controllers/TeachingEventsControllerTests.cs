﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventsControllerTests
    {
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IStore> _mockStore;
        private readonly TeachingEventsController _controller;

        public TeachingEventsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new TeachingEventsController(_mockStore.Object, _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(TeachingEventsController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public async void AddAttendee_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventRegistrationRequest() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = await _controller.AddAttendee(Guid.NewGuid(), request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public async void AddAttendee_MissingEvent_RespondsWithNotFound()
        {
            var request = new TeachingEventRegistrationRequest() { FirstName = null };
            var teachingEventId = Guid.NewGuid();
            _mockStore.Setup(mock => mock.GetTeachingEventAsync(teachingEventId)).ReturnsAsync(null as TeachingEvent);

            var response = await _controller.AddAttendee(teachingEventId, request);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void AddAttendee_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var request = new TeachingEventRegistrationRequest() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            _mockStore.Setup(mock => mock.GetTeachingEventAsync((Guid)teachingEvent.Id)).ReturnsAsync(teachingEvent);

            var response = await _controller.AddAttendee((Guid)teachingEvent.Id, request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(TeachingEventRegistrationJob) && job.Method.Name == "Run" &&
                                  ((TeachingEventRegistrationRequest)job.Args[0]) == request &&
                                  ((Guid)job.Args[1]) == teachingEvent.Id),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async void Search_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventSearchRequest() { Postcode = null };
            _controller.ModelState.AddModelError("Postcode", "Postcode must be specified.");

            var response = await _controller.Search(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Postcode").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Postcode must be specified.");
        }

        [Fact]
        public async void GetUpcoming_ValidRequest_ReturnsTeachingEvents()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "KY12 8FG" };
            var mockEvents = MockEvents();
            _mockStore.Setup(mock => mock.SearchTeachingEventsAsync(request)).ReturnsAsync(mockEvents);

            var response = await _controller.Search(request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEvents);
        }

        [Fact]
        public async void GetUpcoming_LimitMoreThan50_RespondsWithBadRequest()
        {
            var response = await _controller.GetUpcoming(51);

            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void GetUpcoming_ReturnsUpcomingTeachingEvents()
        {
            var mockEvents = MockEvents();
            _mockStore.Setup(mock => mock.GetUpcomingTeachingEvents(3)).Returns(mockEvents.AsAsyncQueryable());

            var response = await _controller.GetUpcoming(3);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEvents);
        }

        [Fact]
        public async void Get_ReturnsTeachingEvents()
        {
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            _mockStore.Setup(mock => mock.GetTeachingEventAsync((Guid)teachingEvent.Id)).ReturnsAsync(teachingEvent);

            var response = await _controller.Get((Guid)teachingEvent.Id);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(teachingEvent);
        }

        [Fact]
        public async void Get_WithMissingEvent_ReturnsNotFound()
        {
            _mockStore.Setup(mock => mock.GetTeachingEventAsync(It.IsAny<Guid>())).ReturnsAsync(null as TeachingEvent);

            var response = await _controller.Get(Guid.NewGuid());

            response.Should().BeOfType<NotFoundResult>();
        }

        private static IEnumerable<TeachingEvent> MockEvents()
        {
            var event1 = new TeachingEvent() { Name = "Event 1" };
            var event2 = new TeachingEvent() { Name = "Event 2" };
            var event3 = new TeachingEvent() { Name = "Event 3" };

            return new[] { event1, event2, event3 };
        }
    }
}
