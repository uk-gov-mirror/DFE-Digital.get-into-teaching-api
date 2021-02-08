﻿using System;
using System.Linq;
using System.Security.Cryptography;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public class CandidateMagicLinkTokenService : ICandidateMagicLinkTokenService
    {
        public static readonly TimeSpan TokenTimeSpan = new TimeSpan(48, 0, 0);
        private readonly ICrmService _crm;
        private readonly RNGCryptoServiceProvider _cryptoService;

        public CandidateMagicLinkTokenService(ICrmService crm)
        {
            _cryptoService = new RNGCryptoServiceProvider();
            _crm = crm;
        }

        public void GenerateToken(Candidate candidate)
        {
            candidate.MagicLinkToken = CreateToken();
            candidate.MagicLinkTokenExpiresAt = DateTime.UtcNow.AddHours(TokenTimeSpan.TotalHours);
        }

        public Candidate Exchange(string token)
        {
            var matchingCandidates = _crm.MatchCandidates(token);

            // Return null if there are no matches and also in the very
            // unlikely case a token has been duplicated.
            if (matchingCandidates.Count() != 1)
            {
                return null;
            }

            return matchingCandidates.First();
        }

        private string CreateToken()
        {
            byte[] bytes = new byte[16];
            _cryptoService.GetBytes(bytes);

            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
    }
}
