using Azure;
using Azure.Data.Tables;
using System;

namespace DashboardApi.Models
{
    public class AggregatedPageViewsEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string TenantId { get; set; }
        public DateTime Date { get; set; }
        public int PageViewCount { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public AggregatedPageViewsEntity() { }

        public AggregatedPageViewsEntity(string tenantId, DateTime date, int pageViewCount)
        {
            TenantId = tenantId;
            Date = date;
            PageViewCount = pageViewCount;
            PartitionKey = tenantId; 
            RowKey = date.ToString("yyyyMMdd"); 
        }

    }
}
