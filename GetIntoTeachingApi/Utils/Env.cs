﻿using System;

namespace GetIntoTeachingApi.Utils
{
    public class Env : IEnv
    {
        public bool IsDevelopment => EnvironmentName == "Development";
        public bool IsProduction => EnvironmentName == "Production";
        public bool IsStaging => EnvironmentName == "Staging";
        public bool IsTest => EnvironmentName == "Test";
        public string GitCommitSha => Environment.GetEnvironmentVariable("GIT_COMMIT_SHA");
        public bool ExportHangireToPrometheus => Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX") == "0";
        public string DatabaseInstanceName => Environment.GetEnvironmentVariable("DATABASE_INSTANCE_NAME");
        public string HangfireInstanceName => Environment.GetEnvironmentVariable("HANGFIRE_INSTANCE_NAME");
        public string TotpSecretKey => Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");
        public string VcapServices => Environment.GetEnvironmentVariable("VCAP_SERVICES");
        public string CrmServiceUrl => Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        public string CrmClientId => Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        public string CrmClientSecret => Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        public string NotifyApiKey => Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
        public string GoogleApiKey => Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

        // The master instance boots first on deploy.
        public bool IsMasterInstance
        {
            get
            {
                var index = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");
                var success = int.TryParse(index, out int value);

                return !success || value == 0;
            }
        }

        public string EnvironmentName
        {
            get
            {
                var name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return name ?? "Test";
            }
        }

        public string Get(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
    }
}
