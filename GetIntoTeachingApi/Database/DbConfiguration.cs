﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApi.Database
{
    public class DbConfiguration
    {
        private const int BufferFlushInterval = 1000;
        private readonly GetIntoTeachingDbContext _dbContext;

        public DbConfiguration(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Configure()
        { 
            _dbContext.Database.Migrate();

            SeedLocations();
        }

        private void SeedLocations()
        {
            if (_dbContext.Locations.Any()) return;

            var buffer = new List<Location>();
            using var reader = new StreamReader(LocationsFixturePath());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var location = CreateLocation(csv);

                if (location.IsNonGeographic()) continue;

                buffer.Add(location);

                FlushBuffer(buffer, _dbContext.Locations);
            }

            FlushBuffer(buffer, _dbContext.Locations, true);
        }

        private static Location CreateLocation(CsvReader csv)
        {
            return new Location()
            {
                Postcode = LocationService.Sanitize(csv.GetField<string>("postcode")),
                Latitude = csv.GetField<double?>("latitude"),
                Longitude = csv.GetField<double?>("longitude")
            };
        }

        private void FlushBuffer<T>(ICollection<T> buffer, DbSet<T> dbSet, bool force = false) where T : class
        {
            if (!force && buffer.Count() != BufferFlushInterval)
                return;

            dbSet.AddRange(buffer);
            _dbContext.SaveChanges();
            buffer.Clear();
        }

        private static string LocationsFixturePath() =>
            Env.IsDevelopment ? "./Fixtures/ukpostcodes.dev.csv" : "./Fixtures/ukpostcodes.csv";
    }
}
