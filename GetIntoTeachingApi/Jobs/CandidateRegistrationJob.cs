﻿using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class CandidateRegistrationJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly ILogger<CandidateRegistrationJob> _logger;

        public CandidateRegistrationJob(
            IEnv env,
            ICrmService crm,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            ILogger<CandidateRegistrationJob> logger)
            : base(env)
        {
            _crm = crm;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
            _logger = logger;
        }

        public void Run(Candidate candidate, PerformContext context)
        {
            _logger.LogInformation($"CandidateRegistrationJob - Started ({AttemptInfo(context, _contextAdapter)})");

            if (IsLastAttempt(context, _contextAdapter))
            {
                var personalisation = new Dictionary<string, dynamic>();

                // We fire and forget the email, ensuring the job succeeds.
                _notifyService.SendEmailAsync(
                    candidate.Email,
                    NotifyService.CandidateRegistrationFailedEmailTemplateId,
                    personalisation);
                _logger.LogInformation("CandidateRegistrationJob - Deleted");
            }
            else
            {
                AddSubscription(candidate);
                _crm.Save(candidate);
                _logger.LogInformation("CandidateRegistrationJob - Succeeded");
            }
        }

        private void AddSubscription(Candidate candidate)
        {
            var alreadySubscribed = candidate.Id != null && _crm.IsCandidateSubscribedToServiceOfType(
                (Guid)candidate.Id, (int)Subscription.ServiceType.TeacherTrainingAdviser);

            if (alreadySubscribed)
            {
                return;
            }

            var subscription = new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.TeacherTrainingAdviser,
            };

            candidate.Subscriptions.Add(subscription);
        }
    }
}
