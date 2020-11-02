﻿using System.Linq;
using System.Threading.Tasks;
using GeocodeSharp.Google;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Adapters
{
    public class GeocodeClientAdapter : IGeocodeClientAdapter
    {
        private readonly IMetricService _metrics;
        private readonly GeocodeClient _client;
        private readonly ILogger<IGeocodeClientAdapter> _logger;

        public GeocodeClientAdapter(IEnv env, IMetricService metrics, ILogger<IGeocodeClientAdapter> logger)
        {
            _metrics = metrics;
            _client = new GeocodeClient(env.GoogleApiKey);
            _logger = logger;
        }

        public async Task<Point> GeocodePostcodeAsync(string postcode)
        {
            var response = await _client.GeocodeAddress(postcode);

            _logger.LogInformation($"Google API Status: {response.StatusText}");

            if (response.Status != GeocodeStatus.Ok)
            {
                _metrics.GoogleApiCalls.WithLabels(postcode, "error").Inc();
                return null;
            }

            var result = response.Results.FirstOrDefault();

            if (result == null)
            {
                _metrics.GoogleApiCalls.WithLabels(postcode, "fail").Inc();
                return null;
            }

            _metrics.GoogleApiCalls.WithLabels(postcode, "success").Inc();

            var location = result.Geometry.Location;
            return new Point(new Coordinate(location.Longitude, location.Latitude));
        }
    }
}
