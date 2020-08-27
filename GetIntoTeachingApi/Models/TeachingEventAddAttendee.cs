﻿using System;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventAddAttendee
    {
        public Guid? CandidateId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? EventId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToEvents { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

        public TeachingEventAddAttendee()
        {
        }

        public TeachingEventAddAttendee(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            CandidateId = candidate.Id;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressPostcode = candidate.AddressPostcode;
            Telephone = candidate.Telephone;

            AlreadySubscribedToMailingList = candidate.HasMailingListSubscription == true;
            AlreadySubscribedToEvents = candidate.HasEventsSubscription == true;
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.Event : null,
            };

            AddTeachingEventRegistration(candidate);
            AcceptPrivacyPolicy(candidate);
            ConfigureSubscriptions(candidate);
            ConfigureConsent(candidate);

            return candidate;
        }

        private void ConfigureConsent(Candidate candidate)
        {
            candidate.OptOutOfSms = false;
            candidate.DoNotBulkEmail = !SubscribeToEvents && !SubscribeToMailingList;
            candidate.DoNotBulkPostalMail = !SubscribeToMailingList;
            candidate.DoNotEmail = false;
            candidate.DoNotPostalMail = !SubscribeToMailingList;
            candidate.DoNotSendMm = !SubscribeToEvents && !SubscribeToMailingList;
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            candidate.HasEventsSubscription = true;
            candidate.EventsSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Events;
            candidate.EventsSubscriptionStartAt = DateTime.UtcNow;
            candidate.EventsSubscriptionDoNotEmail = false;
            candidate.EventsSubscriptionDoNotBulkEmail = !SubscribeToEvents;
            candidate.EventsSubscriptionDoNotBulkPostalMail = true;
            candidate.EventsSubscriptionDoNotPostalMail = true;
            candidate.EventsSubscriptionDoNotSendMm = !SubscribeToEvents;

            if (!SubscribeToMailingList)
            {
                return;
            }

            candidate.HasMailingListSubscription = true;
            candidate.MailingListSubscriptionChannelId = (int)Candidate.SubscriptionChannel.MailingList;
            candidate.MailingListSubscriptionStartAt = DateTime.UtcNow;
            candidate.MailingListSubscriptionDoNotEmail = false;
            candidate.MailingListSubscriptionDoNotBulkEmail = false;
            candidate.MailingListSubscriptionDoNotBulkPostalMail = true;
            candidate.MailingListSubscriptionDoNotPostalMail = true;
            candidate.MailingListSubscriptionDoNotSendMm = false;
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
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }
        }
    }
}
