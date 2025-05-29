using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Project.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly TelemetryClient _telemetryClient;

        protected BaseController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        protected void TrackTrace(string message, IDictionary<string, string> properties = null)
        {
            _telemetryClient.TrackTrace(message, properties);
        }

        protected void TrackEvent(string eventName, IDictionary<string, string> properties = null)
        {
            _telemetryClient.TrackEvent(eventName, properties);
        }

        protected void TrackException(Exception exception, IDictionary<string, string> properties = null)
        {
            _telemetryClient.TrackException(exception, properties);
        }

        protected void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            _telemetryClient.TrackMetric(name, value, properties);
        }

        protected void TrackDependency(string dependencyTypeName, string dependencyName, string data, 
            DateTime startTime, TimeSpan duration, bool success)
        {
            _telemetryClient.TrackDependency(
                dependencyTypeName,
                dependencyName,
                data,
                startTime,
                duration,
                success
            );
        }

        protected void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, 
            string responseCode, bool success)
        {
            _telemetryClient.TrackRequest(name, startTime, duration, responseCode, success);
        }
    }
} 