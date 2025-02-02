﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventAddAttendee
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }

        [SwaggerSchema(WriteOnly = true)]
        public Guid? EventId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }

        public int? ConsiderationJourneyStageId { get; set; }
        public int? DegreeStatusId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

        public TeachingEventAddAttendee()
        {
        }

        public TeachingEventAddAttendee(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            var latestQualification = candidate.Qualifications.OrderByDescending(q => q.CreatedAt).FirstOrDefault();

            if (latestQualification != null)
            {
                QualificationId = latestQualification.Id;
                DegreeStatusId = latestQualification.DegreeStatusId;
            }

            CandidateId = candidate.Id;
            PreferredTeachingSubjectId = candidate.PreferredTeachingSubjectId;

            ConsiderationJourneyStageId = candidate.ConsiderationJourneyStageId;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressPostcode = candidate.AddressPostcode;
            Telephone = candidate.Telephone;

            AlreadySubscribedToMailingList = candidate.HasMailingListSubscription == true;
            AlreadySubscribedToEvents = candidate.HasEventsSubscription == true;
            AlreadySubscribedToTeacherTrainingAdviser = candidate.HasTeacherTrainingAdviser();
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                ConsiderationJourneyStageId = ConsiderationJourneyStageId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode.AsFormattedPostcode(),
                Telephone = Telephone,
            };

            ConfigureChannel(candidate);
            AddTeachingEventRegistration(candidate);
            AddQualification(candidate);
            AcceptPrivacyPolicy(candidate);
            ConfigureSubscriptions(candidate);

            return candidate;
        }

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                candidate.ChannelId = (int?)Candidate.Channel.Event;
            }
        }

        private void AddQualification(Candidate candidate)
        {
            if (DegreeStatusId != null)
            {
                candidate.Qualifications.Add(new CandidateQualification()
                {
                    Id = QualificationId,
                    DegreeStatusId = DegreeStatusId,
                    TypeId = (int)CandidateQualification.DegreeType.Degree,
                });
            }
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            var utcNow = DateTimeProvider.UtcNow;

            SubscriptionManager.SubscribeToEvents(candidate, utcNow);

            if (SubscribeToMailingList)
            {
                SubscriptionManager.SubscribeToMailingList(candidate, utcNow);
            }
        }

        private void AddTeachingEventRegistration(Candidate candidate)
        {
            if (EventId != null)
            {
                candidate.TeachingEventRegistrations.Add(new TeachingEventRegistration()
                {
                    EventId = (Guid)EventId,
                    ChannelId = (int)TeachingEventRegistration.Channel.Event,
                    IsCancelled = false,
                    RegistrationNotificationSeen = false,
                });
            }
        }

        private void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy()
                {
                    AcceptedPolicyId = (Guid)AcceptedPolicyId,
                    AcceptedAt = DateTimeProvider.UtcNow,
                };
            }
        }
    }
}
