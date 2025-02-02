﻿using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        string CheckStatus();
        IEnumerable<LookupItem> GetLookupItems(string entityName);
        IEnumerable<PickListItem> GetPickListItems(string entityName, string attributeName);
        IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        Candidate MatchCandidate(ExistingCandidateRequest request);
        IEnumerable<Candidate> MatchCandidates(string magicLinkToken);
        Candidate GetCandidate(Guid id);
        IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10);
        IEnumerable<TeachingEvent> GetTeachingEvents(DateTime? startAfter = null);
        TeachingEvent GetTeachingEvent(string readableId);
        IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas();
        CallbackBookingQuota GetCallbackBookingQuota(DateTime scheduledAt);
        bool CandidateAlreadyHasLocalEventSubscriptionType(Guid candidateId);
        bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId);
        bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId);
        void Save(BaseModel model);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        IEnumerable<Entity> RelatedEntities(Entity entity, string relationshipName, string logicalName);
        Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context);
        IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings();
    }
}