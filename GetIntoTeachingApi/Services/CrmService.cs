﻿using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {
        public enum PrivacyPolicyType { Web = 222750001 }

        private readonly IOrganizationServiceAdapter _service;
        private readonly ICrmCache _cache;
        private const int CacheDurationInHours = 3;
        private const int MaximumNumberOfCandidatesToMatch = 20;
        private const int MaximumNumberOfPrivacyPolicies = 3;

        public CrmService(IOrganizationServiceAdapter service, ICrmCache cache)
        {
            _service = service;
            _cache = cache;
        }

        public IEnumerable<TypeEntity> GetLookupItems(string entityName)
        {
            return _cache.GetOrCreate(entityName, CacheExpiry(), 
                () => _service.CreateQuery(entityName, Context()).Select((entity) => new TypeEntity(entity)));
        }

        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _cache.GetOrCreate(entityName, CacheExpiry(),
                () => _service.GetPickListItemsForAttribute(ConnectionString(), entityName, attributeName)
                    .Select((pickListItem) => new TypeEntity(pickListItem)));
        }

        public PrivacyPolicy GetLatestPrivacyPolicy()
        {
            return GetPrivacyPolicies().FirstOrDefault();
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _cache.GetOrCreate("dfe_privacypolicy", CacheExpiry(), () => 
            {
                return _service.CreateQuery("dfe_privacypolicy", Context())
                    .Where((entity) =>
                        entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicyType.Web &&
                        entity.GetAttributeValue<bool>("dfe_active")
                    )
                    .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                    .Select((entity) => new PrivacyPolicy(entity, this))
                    .Take(MaximumNumberOfPrivacyPolicies);
            });
        }

        public Candidate GetCandidate(ExistingCandidateRequest request)
        {
            var context = Context();
            var entity = _service.CreateQuery("contact", context)
                .Where(e =>
                    // Will perform a case-insensitive comparison
                    e.GetAttributeValue<string>("emailaddress1") == request.Email
                )
                .OrderByDescending(e => e.GetAttributeValue<DateTime>("createdon"))
                .Take(MaximumNumberOfCandidatesToMatch)
                .ToList()
                .FirstOrDefault(request.Match);

            if (entity == null)
                return null;

            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);

            return new Candidate(entity, this);
        }

        public bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId)
        {
            return _service.CreateQuery("dfe_candidateprivacypolicy", Context()).FirstOrDefault(entity => 
                entity.GetAttributeValue<EntityReference>("dfe_candidate").Id == candidateId && 
                entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id == privacyPolicyId) == null;
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _service.AddLink(source, relationship, target, context);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            return _service.RelatedEntities(entity, attributeName);
        }

        public Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context)
        { 
            return id != null ? _service.BlankExistingEntity(entityName, (Guid)id, context) : _service.NewEntity(entityName, context);
        }

        public void Save(BaseModel model)
        {
            using var context = Context();
            model.ToEntity(this, context);
            _service.SaveChanges(context);
        }

        private DateTime CacheExpiry()
        {
            return DateTime.Now.AddHours(CacheDurationInHours);
        }

        private OrganizationServiceContext Context()
        {
            return _service.Context(ConnectionString());
        }

        private static string ConnectionString()
        {
            var instanceUrl = Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
            var clientId = Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
            return $"AuthType=ClientSecret; url={instanceUrl}; ClientId={clientId}; ClientSecret={clientSecret}";
        }
    }
}
