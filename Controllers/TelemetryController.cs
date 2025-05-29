using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : BaseController
    {
        private readonly TelemetryClient _telemetry;

        public TelemetryController(TelemetryClient telemetryClient) 
            : base(telemetryClient)
        {
            _telemetry = telemetryClient;
        }

        [HttpGet("trace")]
        public IActionResult LogTrace(string message = "Default trace message")
        {
            var properties = new Dictionary<string, string>
            {
                { "Source", "TelemetryController" },
                { "Type", "Trace" }
            };
            
            _telemetry.TrackTrace(message, properties);
            return Ok("Trace log recorded successfully");
        }

        [HttpGet("event")]
        public IActionResult LogEvent(string eventName = "CustomEvent")
        {
            var properties = new Dictionary<string, string>
            {
                { "Category", "Testing" },
                { "Source", "TelemetryController" },
                { "Timestamp", DateTime.UtcNow.ToString("o") }
            };

            _telemetry.TrackEvent(eventName, properties);
            return Ok("Event logged successfully");
        }

        [HttpGet("exception")]
        public IActionResult LogException(string message = "Test exception")
        {
            try
            {
                throw new InvalidOperationException(message);
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    { "Source", "TelemetryController" },
                    { "ExceptionType", ex.GetType().Name }
                };

                _telemetry.TrackException(ex, properties);
                return Ok("Exception logged successfully");
            }
        }

        [HttpGet("metric")]
        public IActionResult LogMetric(string name = "TestMetric", double value = 1.0)
        {
            var properties = new Dictionary<string, string>
            {
                { "Source", "TelemetryController" },
                { "Type", "CustomMetric" }
            };

            _telemetry.TrackMetric(name, value, properties);
            return Ok("Metric logged successfully");
        }

        [HttpGet("dependency")]
        public IActionResult LogDependency()
        {
            var startTime = DateTime.UtcNow;
            var duration = TimeSpan.FromMilliseconds(150); // Simulated duration

            _telemetry.TrackDependency(
                "HTTP",
                "ExternalAPI",
                "GET /api/data",
                startTime,
                duration,
                true
            );

            return Ok("Dependency logged successfully");
        }

        [HttpGet("request")]
        public IActionResult LogCustomRequest()
        {
            var startTime = DateTimeOffset.UtcNow;
            var duration = TimeSpan.FromMilliseconds(100); // Simulated duration

            _telemetry.TrackRequest(
                "CustomRequest",
                startTime,
                duration,
                "200",
                true
            );

            return Ok("Request logged successfully");
        }

        [HttpGet("all")]
        public IActionResult LogAllTypes()
        {
            // Log a trace
            _telemetry.TrackTrace("Testing all telemetry types");

            // Log an event
            _telemetry.TrackEvent("TestAllEvent", new Dictionary<string, string> { { "Type", "All" } });

            // Log a metric
            _telemetry.TrackMetric("TestAllMetric", 1.0);

            // Log a dependency
            _telemetry.TrackDependency(
                "Database",
                "TestDB",
                "SELECT * FROM Test",
                DateTime.UtcNow,
                TimeSpan.FromMilliseconds(50),
                true
            );

            // Log a request
            _telemetry.TrackRequest(
                "TestAllRequest",
                DateTimeOffset.UtcNow,
                TimeSpan.FromMilliseconds(75),
                "200",
                true
            );

            return Ok("All telemetry types logged successfully");
        }
    }
} 