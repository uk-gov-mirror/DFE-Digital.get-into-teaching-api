﻿using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidatePastTeachingPositionTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CandidatePastTeachingPosition);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_candidatepastteachingposition");

            type.GetProperty("SubjectTaughtId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_subjecttaught" && a.Type == typeof(EntityReference) && a.Reference == "dfe_teachingsubjectlist");

            type.GetProperty("EducationPhaseId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_educationphase" && a.Type == typeof(OptionSetValue));

            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "createdon");
        }
    }
}