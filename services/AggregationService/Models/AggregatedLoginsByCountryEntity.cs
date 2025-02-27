using Azure;
using Microsoft.WindowsAzure.Storage;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace AggregationServiceApp.Models
{
    public class AggregatedLoginsByCountryEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string TenantId { get; set; }
        public string Country { get; set; }
        public int LoginCount { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public AggregatedLoginsByCountryEntity() { }

        public AggregatedLoginsByCountryEntity(string tenantId, string country, int loginCount)
        {
            TenantId = tenantId;
            Country = country;
            LoginCount = loginCount;
            PartitionKey = tenantId;  
            RowKey = country;           
        }
    }
}
