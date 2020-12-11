﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateValidator : AbstractValidator<Candidate>
    {
        private readonly IStore _store;
        private readonly string[] _validEligibilityRulesPassedValues = new[] { "true", "false" };

        public CandidateValidator(IStore store)
        {
            _store = store;

            RuleFor(candidate => candidate.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.LastName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(candidate => candidate.DateOfBirth).LessThan(candidate => DateTime.UtcNow);
            RuleFor(candidate => candidate.Telephone).MinimumLength(5).MaximumLength(20).Matches(@"^[^a-zA-Z]+$");
            RuleFor(candidate => candidate.AddressLine1).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressLine2).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressCity).MaximumLength(128);
            RuleFor(candidate => candidate.AddressPostcode).MaximumLength(40).Matches(Location.PostcodeRegex);
            RuleFor(candidate => candidate.EligibilityRulesPassed)
                .Must(value => _validEligibilityRulesPassedValues.Contains(value))
                .Unless(candidate => candidate.EligibilityRulesPassed == null)
                .WithMessage("Must be true or false (as string values).");

            RuleFor(candidate => candidate.PhoneCall).SetValidator(new PhoneCallValidator(store)).Unless(candidate => candidate.PhoneCall == null);
            RuleFor(candidate => candidate.PrivacyPolicy).NotNull().SetValidator(new CandidatePrivacyPolicyValidator(store));
            RuleForEach(candidate => candidate.Qualifications).SetValidator(new CandidateQualificationValidator(store));
            RuleForEach(candidate => candidate.PastTeachingPositions).SetValidator(new CandidatePastTeachingPositionValidator(store));
            RuleForEach(candidate => candidate.TeachingEventRegistrations).SetValidator(new TeachingEventRegistrationValidator(store));
            RuleFor(candidate => candidate.TeachingEventRegistrations)
                .Must(registrations => registrations.Select(s => s.EventId).Distinct().Count() == registrations.Count)
                .WithMessage("Must not contain multiple registrations for the same event.");

            RuleFor(candidate => candidate.PreferredTeachingSubjectId)
                .Must(id => PreferredTeachingSubjectIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PreferredTeachingSubjectId == null)
                .WithMessage("Must be a valid teaching subject.");
            RuleFor(candidate => candidate.CountryId)
                .Must(id => CountryIds().Contains(id.ToString()))
                .Unless(candidate => candidate.CountryId == null)
                .WithMessage("Must be a valid country.");
            RuleFor(candidate => candidate.PreferredEducationPhaseId)
                .Must(id => PreferredEducationPhaseIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PreferredEducationPhaseId == null)
                .WithMessage("Must be a valid candidate education phase.");
            RuleFor(candidate => candidate.InitialTeacherTrainingYearId)
                .Must(id => InitialTeacherTrainingYearIds().Contains(id.ToString()))
                .Unless(candidate => candidate.InitialTeacherTrainingYearId == null)
                .WithMessage("Must be a valid candidate initial teacher training year.");
            RuleFor(candidate => candidate.ChannelId)
                .Must(id => ChannelIds().Contains(id.ToString()))
                .Unless(candidate => candidate.Id != null)
                .WithMessage("Must be a valid candidate channel.");
            RuleFor(candidate => candidate.ChannelId)
                .Must(id => id == null)
                .Unless(candidate => candidate.Id == null)
                .WithMessage("You cannot change the channel of an existing candidate.");
            RuleFor(candidate => candidate.HasGcseEnglishId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseEnglishId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
            RuleFor(candidate => candidate.HasGcseMathsId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseMathsId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
            RuleFor(candidate => candidate.HasGcseScienceId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseScienceId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
            RuleFor(candidate => candidate.PlanningToRetakeGcseScienceId)
                .Must(id => RetakeGcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PlanningToRetakeGcseScienceId == null)
                .WithMessage("Must be a valid candidate retake GCSE status.");
            RuleFor(candidate => candidate.PlanningToRetakeGcseEnglishId)
                .Must(id => RetakeGcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PlanningToRetakeGcseEnglishId == null)
                .WithMessage("Must be a valid candidate retake GCSE status.");
            RuleFor(candidate => candidate.PlanningToRetakeGcseMathsId)
                .Must(id => RetakeGcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PlanningToRetakeGcseMathsId == null)
                .WithMessage("Must be a valid candidate retake GCSE status.");
            RuleFor(candidate => candidate.ConsiderationJourneyStageId)
                .Must(id => ConsiderationJourneyStageIds().Contains(id.ToString()))
                .Unless(candidate => candidate.ConsiderationJourneyStageId == null)
                .WithMessage("Must be a valid candidate consideration journey stage.");
            RuleFor(candidate => candidate.TypeId)
                .Must(id => TypeIds().Contains(id.ToString()))
                .Unless(candidate => candidate.TypeId == null)
                .WithMessage("Must be a valid candidate type.");
            RuleFor(candidate => candidate.AssignmentStatusId)
                .Must(id => AssignmentStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.AssignmentStatusId == null)
                .WithMessage("Must be a valid candidate assignment status.");
            RuleFor(candidate => candidate.AdviserEligibilityId)
                .Must(id => AdviserEligibilityIds().Contains(id.ToString()))
                .Unless(candidate => candidate.AdviserEligibilityId == null)
                .WithMessage("Must be a valid candidate adviser eligibility.");
            RuleFor(candidate => candidate.AdviserRequirementId)
                .Must(id => AdviserRequirementIds().Contains(id.ToString()))
                .Unless(candidate => candidate.AdviserRequirementId == null)
                .WithMessage("Must be a valid candidate adviser requirement.");
            RuleFor(candidate => candidate.EventsSubscriptionChannelId)
                .Must(id => EventSubscriptionChannelIds().Contains(id.ToString()))
                .Unless(candidate => candidate.EventsSubscriptionChannelId == null)
                .WithMessage("Must be a valid event subscription channel.");
            RuleFor(candidate => candidate.MailingListSubscriptionChannelId)
                .Must(id => MailingListSubscriptionChannelIds().Contains(id.ToString()))
                .Unless(candidate => candidate.MailingListSubscriptionChannelId == null)
                .WithMessage("Must be a valid mailing list subscription channel.");
        }

        private IEnumerable<string> PreferredTeachingSubjectIds()
        {
            return _store.GetTypeEntitites("dfe_teachingsubjectlist").Select(subject => subject.Id);
        }

        private IEnumerable<string> CountryIds()
        {
            return _store.GetTypeEntitites("dfe_country").Select(country => country.Id);
        }

        private IEnumerable<string> PreferredEducationPhaseIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_preferrededucationphase01").Select(phase => phase.Id);
        }

        private IEnumerable<string> InitialTeacherTrainingYearIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_ittyear").Select(year => year.Id);
        }

        private IEnumerable<string> ChannelIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_channelcreation").Select(channel => channel.Id);
        }

        private IEnumerable<string> MailingListSubscriptionChannelIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_gitismlservicesubscriptionchannel").Select(channel => channel.Id);
        }

        private IEnumerable<string> EventSubscriptionChannelIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_gitiseventsservicesubscriptionchannel").Select(channel => channel.Id);
        }

        private IEnumerable<string> GcseStatusIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_websitehasgcseenglish").Select(status => status.Id);
        }

        private IEnumerable<string> RetakeGcseStatusIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_websiteplanningretakeenglishgcse").Select(status => status.Id);
        }

        private IEnumerable<string> ConsiderationJourneyStageIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_websitewhereinconsiderationjourney").Select(describe => describe.Id);
        }

        private IEnumerable<string> TypeIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_typeofcandidate").Select(type => type.Id);
        }

        private IEnumerable<string> AssignmentStatusIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_candidatestatus").Select(type => type.Id);
        }

        private IEnumerable<string> AdviserEligibilityIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_iscandidateeligibleforadviser").Select(eligibility => eligibility.Id);
        }

        private IEnumerable<string> AdviserRequirementIds()
        {
            return _store.GetTypeEntitites("contact", "dfe_isadvisorrequiredos").Select(requirement => requirement.Id);
        }
    }
}