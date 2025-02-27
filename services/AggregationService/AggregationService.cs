using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TrackingServices.Models;
using AggregationServiceApp.Models;
using System.Globalization;

public class AggregationService
{
    private readonly TableClient _pageViewsTableClient;
    private readonly TableClient _loginsTableClient;

    public AggregationService(TableClient pageViewsTableClient, TableClient loginsTableClient)
    {
        _pageViewsTableClient = pageViewsTableClient;
        _loginsTableClient = loginsTableClient;
    }

    public async Task ProcessUserActivityEventAsync(ServiceBusReceivedMessage message, ILogger log)
    {
        try
        {
            // Deserialize the message body into UserActivityEvent
            var activityEvent = JsonConvert.DeserializeObject<UserActivityEvent>(message.Body.ToString());

            if (activityEvent.EventType == "PageView")
            {
                // Aggregate page views per day
                await AggregatePageViewsPerDayAsync(activityEvent, log);
            }
            else if (activityEvent.EventType == "Login")
            {
                // Aggregate logins by country
                await AggregateLoginsByCountryAsync(activityEvent, log);
            }
            else
            {
                log.LogInformation($"Event type {activityEvent.EventType} not supported.");
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing event: {ex.Message}");
        }
    }

    public async Task AggregatePageViewsPerDayAsync(UserActivityEvent activityEvent, ILogger log)
    {
        try
        {
            var date = activityEvent.Timestamp.Date; 
            string dateString = date.ToString("yyyyMMdd");
            DateTime parsedDate = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);

            var key = $"{activityEvent.TenantId}_{date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}";


            // Store the page view count in a dictionary
            var pageViewCounts = new Dictionary<string, int>();
            if (!pageViewCounts.ContainsKey(key))
            {
                pageViewCounts[key] = 0;
            }
            pageViewCounts[key]++;

            // Store the aggregated data in Table Storage
            foreach (var item in pageViewCounts)
            {
                var tenantId = item.Key.Split('_')[0];
                var dateParsed = DateTime.ParseExact(item.Key.Split('_')[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                var utcDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                var entity = new AggregatedPageViewsEntity(tenantId, utcDate, item.Value);

                // Add the entity to Table Storage
                await _pageViewsTableClient.AddEntityAsync(entity);
            }

            log.LogInformation($"Page view aggregation completed for tenant: {activityEvent.TenantId} on {date.ToString("yyyyMMdd")}");
        }
        catch (Exception ex)
        {
            log.LogError($"Error aggregating page views: {ex.Message}");
        }
    }

    public async Task AggregateLoginsByCountryAsync(UserActivityEvent activityEvent, ILogger log)
    {
        try
        {
            var key = $"{activityEvent.TenantId}_{activityEvent.Country}";

            // Store the login count in a dictionary
            var loginCountsByCountry = new Dictionary<string, int>();
            if (!loginCountsByCountry.ContainsKey(key))
            {
                loginCountsByCountry[key] = 0;
            }
            loginCountsByCountry[key]++;

            // Store the aggregated data in Table Storage
            foreach (var item in loginCountsByCountry)
            {
                var tenantId = item.Key.Split('_')[0];
                var country = item.Key.Split('_')[1];

                var entity = new AggregatedLoginsByCountryEntity(tenantId, country, item.Value);

                // Add the entity to Table Storage
                await _loginsTableClient.AddEntityAsync(entity);
            }

            log.LogInformation($"Login aggregation completed for tenant: {activityEvent.TenantId} in country: {activityEvent.Country}");
        }
        catch (Exception ex)
        {
            log.LogError($"Error aggregating logins: {ex.Message}");
        }
    }
}
