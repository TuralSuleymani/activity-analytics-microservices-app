using Microsoft.AspNetCore.Mvc;
using Azure.Data.Tables;
using DashboardApi.Models;

namespace DashboardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly TableClient _pageViewsTableClient;
        private readonly TableClient _loginsTableClient;
        private readonly ILogger<DashboardController> _log;

        public DashboardController(TableClient pageViewsTableClient, TableClient loginsTableClient, ILogger<DashboardController> log)
        {
            _pageViewsTableClient = pageViewsTableClient;
            _loginsTableClient = loginsTableClient;
            _log = log;
        }

        // Example: Get page views per day
        [HttpGet("pageviews")]
        public async Task<IActionResult> GetPageViewsAsync(string tenantId, string startDate, string endDate)
        {
            _log.LogInformation($"Fetching page views for tenant {tenantId} from {startDate} to {endDate}");

            // Fetch aggregated page views from Table Storage
            var query = _pageViewsTableClient.Query<AggregatedPageViewsEntity>(entity => entity.TenantId == tenantId &&
                entity.Timestamp >= DateTime.Parse(startDate) && entity.Timestamp <= DateTime.Parse(endDate));

            return Ok(query);
        }

        // Example: Get logins by country
        [HttpGet("logins")]
        public async Task<IActionResult> GetLoginsAsync(string tenantId, string startDate, string endDate)
        {
            _log.LogInformation($"Fetching logins for tenant {tenantId} from {startDate} to {endDate}");

            // Fetch aggregated logins from Table Storage
            var query = _loginsTableClient.Query<AggregatedLoginsByCountryEntity>(entity => entity.TenantId == tenantId &&
                entity.Timestamp >= DateTime.Parse(startDate) && entity.Timestamp <= DateTime.Parse(endDate));

            return Ok(query);
        }

        // Example: Get combined overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverviewAsync(string tenantId)
        {
            // Get combined data: page views and logins
            var pageViews = await GetPageViewsAsync(tenantId, "2025-01-01", "2025-12-31");
            var logins = await GetLoginsAsync(tenantId, "2025-01-01", "2025-12-31");

            return Ok(new
            {
                PageViews = pageViews,
                Logins = logins
            });
        }
    }
}
