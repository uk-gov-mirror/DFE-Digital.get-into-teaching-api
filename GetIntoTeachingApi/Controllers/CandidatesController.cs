﻿using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching,GetAnAdviser")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _accessTokenService;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;
        private readonly IAppSettings _appSettings;

        public CandidatesController(
            ICandidateAccessTokenService accessTokenService,
            INotifyService notifyService,
            ICrmService crm,
            IAppSettings appSettings)
        {
            _accessTokenService = accessTokenService;
            _notifyService = notifyService;
            _crm = crm;
            _appSettings = appSettings;
        }

        [HttpPost]
        [Route("access_tokens")]
        [SwaggerOperation(
            Summary = "Creates a candidate access token.",
            Description = @"
                Finds a candidate matching at least 3 of the provided CandidateAccessTokenRequest attributes (including email). 
                If a candidate is found, an access token (PIN code) will be sent to the candidate email address 
                that can then be used for verification.",
            OperationId = "CreateCandidateAccessToken",
            Tags = new[] { "Candidates" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult CreateAccessToken([FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            if (_appSettings.IsCrmIntegrationPaused)
            {
                return NotFound();
            }

            var candidate = _crm.MatchCandidate(request);
            if (candidate == null)
            {
                return NotFound();
            }

            var token = _accessTokenService.GenerateToken(request, (Guid)candidate.Id);
            var personalisation = new Dictionary<string, dynamic> { { "pin_code", token } };

            // We respond immediately/assume this will be successful.
            _notifyService.SendEmailAsync(request.Email, NotifyService.NewPinCodeEmailTemplateId, personalisation);

            return NoContent();
        }
    }
}
