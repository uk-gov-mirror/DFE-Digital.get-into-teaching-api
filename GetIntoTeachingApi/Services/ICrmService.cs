﻿using System;
using GetIntoTeachingApi.Models;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        public IEnumerable<TypeEntity> GetLookupItems(string entityName);
        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        public Candidate GetCandidate(ExistingCandidateRequest request);
        public IEnumerable<TeachingEvent> GetTeachingEvents();
        public bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId);
        public bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId);
        public void Save(BaseModel model);
        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName, string logicalName);
        public Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context);
    }
}
