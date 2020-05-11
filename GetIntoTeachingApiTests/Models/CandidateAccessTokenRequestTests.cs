﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateAccessTokenRequestTests
    {
        private readonly CandidateAccessTokenRequest _request;

        public CandidateAccessTokenRequestTests()
        {
            _request = new CandidateAccessTokenRequest
            {
                Email = "email@address.com",
                FirstName = "first",
                LastName = "last",
                DateOfBirth = new DateTime(2000, 1, 1)
            };
        }

        [Fact]
        public void Match_WithNullCandidate_ReturnsFalse()
        {
            _request.Match(null).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndNoAdditionalAttributes_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                Email = "email@address.com",
            };

            _request.Match(candidate).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndOneAdditionalAttribute_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                Email = _request.Email,
                FirstName = _request.FirstName
            };

            _request.Match(candidate).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndTwoAdditionalAttributes_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                Email = _request.Email,
                FirstName = _request.FirstName,
                LastName = _request.LastName
            };

            _request.Match(candidate).Should().BeTrue();
        }

        [Fact]
        public void Match_WithoutEmailAndWithTwoAdditionalAttributes_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                Email = "wrong@email.com",
                FirstName = _request.FirstName,
                LastName = _request.LastName
            };

            _request.Match(candidate).Should().BeFalse();
        }

        [Fact]
        public void Match_WithWrongEmailAndWithThreeAdditionalAttributes_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                Email = "wrong@email.com",
                FirstName = _request.FirstName,
                LastName = _request.LastName,
                DateOfBirth = _request.DateOfBirth
            };

            _request.Match(candidate).Should().BeFalse();
        }

        [Fact]
        public void Match_WithNullEmailAndWithThreeAdditionalAttributes_ReturnsFalse()
        {
            var candidate = new Candidate
            {
                Email = _request.Email,
                FirstName = _request.FirstName,
                LastName = _request.LastName,
                DateOfBirth = _request.DateOfBirth
            };
            _request.Email = null;

            _request.Match(candidate).Should().BeFalse();
        }

        [Fact]
        public void Match_WithEmailAndThreeAdditionalAttributes_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                Email = _request.Email,
                FirstName = _request.FirstName,
                LastName = _request.LastName,
                DateOfBirth = _request.DateOfBirth
            };

            _request.Match(candidate).Should().BeTrue();
        }

        [Fact]
        public void Match_WithCaseInsensitiveMatch_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                Email = _request.Email.ToUpper(),
                FirstName = _request.FirstName.ToUpper(),
                LastName = _request.LastName.ToUpper(),
            };

            _request.Match(candidate).Should().BeTrue();
        }

        [Fact]
        public void Match_WithMatchingDateButDifferentTimes_ReturnsTrue()
        {
            var candidate = new Candidate
            {
                Email = _request.Email,
                FirstName = _request.FirstName,
                DateOfBirth = _request.DateOfBirth?.AddMinutes(30),
            };

            _request.Match(candidate).Should().BeTrue();
        }
    }
}
