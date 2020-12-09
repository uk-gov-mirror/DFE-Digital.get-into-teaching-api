﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Loggable]
    public abstract class AddCandidate
    {
        public Guid? CandidateId { get; set; }
        public Guid? QualificationId { get; set; }

        public Guid? PreferredTeachingSubjectId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }
        public int? ConsiderationJourneyStageId { get; set; }
        public int? DegreeStatusId { get; set; }

        [SensitiveData]
        public string Email { get; set; }
        [SensitiveData]
        public string FirstName { get; set; }
        [SensitiveData]
        public string LastName { get; set; }
        [SensitiveData]
        public string AddressPostcode { get; set; }
        [SensitiveData]
        public string Telephone { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToTeacherTrainingAdviser { get; set; }

        [JsonIgnore]
        public abstract Candidate Candidate { get; }

        protected virtual Candidate CreateCandidate()
        {
            return new Candidate()
            {
                Id = CandidateId,
                ConsiderationJourneyStageId = ConsiderationJourneyStageId,
                PreferredTeachingSubjectId = PreferredTeachingSubjectId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
            };
        }

        protected void PopulateWithCandidate(Candidate candidate)
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

        protected void ConfigureEventsSubscriptions(Candidate candidate)
        {
            candidate.HasEventsSubscription = true;
            candidate.EventsSubscriptionStartAt = DateTime.UtcNow;
            candidate.EventsSubscriptionDoNotEmail = false;
            candidate.EventsSubscriptionDoNotBulkEmail = false;
            candidate.EventsSubscriptionDoNotBulkPostalMail = true;
            candidate.EventsSubscriptionDoNotPostalMail = true;
            candidate.EventsSubscriptionDoNotSendMm = false;
        }

        protected void ConfigureMailingListSubscriptions(Candidate candidate)
        {
            candidate.HasMailingListSubscription = true;
            candidate.MailingListSubscriptionStartAt = DateTime.UtcNow;
            candidate.MailingListSubscriptionDoNotEmail = false;
            candidate.MailingListSubscriptionDoNotBulkEmail = false;
            candidate.MailingListSubscriptionDoNotBulkPostalMail = true;
            candidate.MailingListSubscriptionDoNotPostalMail = true;
            candidate.MailingListSubscriptionDoNotSendMm = false;
        }

        protected void AddQualification(Candidate candidate)
        {
            candidate.Qualifications.Add(new CandidateQualification()
            {
                Id = QualificationId,
                DegreeStatusId = DegreeStatusId,
                TypeId = (int)CandidateQualification.DegreeType.Degree,
            });
        }

        protected void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }
        }

        protected virtual void ConfigureConsent(Candidate candidate)
        {
            candidate.OptOutOfSms = false;
            candidate.DoNotBulkEmail = false;
            candidate.DoNotEmail = false;
        }
    }
}