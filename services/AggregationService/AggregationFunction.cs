using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Messaging.ServiceBus;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;

public static class AggregationFunction
{
    private static string KeyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");
    private static TableClient pageViewsTableClient;
    private static TableClient loginsTableClient;
    private static AggregationService aggregationService;

    static AggregationFunction()
    {
        var secretClient = new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential());
        var tableStorageConnectionStringSecret = secretClient.GetSecret("AzureTableStorageConnectionString").Value.Value;
        var serviceBusConnectionStringSecret = secretClient.GetSecret("ServiceBusConnectionString").Value.Value;

        pageViewsTableClient = new TableClient(tableStorageConnectionStringSecret, "AggregatedPageViews");
        loginsTableClient = new TableClient(tableStorageConnectionStringSecret, "AggregatedLoginsByCountry");
        
        Environment.SetEnvironmentVariable("ServiceBusConnectionString", serviceBusConnectionStringSecret);

        aggregationService = new AggregationService(pageViewsTableClient, loginsTableClient);
    }

    [FunctionName("ProcessUserActivityEvent")]
    public static async Task Run(
        [ServiceBusTrigger("user-activity-events", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
        ILogger log)
    {
        log.LogInformation($"Service Bus message received at: {DateTime.UtcNow}");

        try
        {
            // Process the message with AggregationService
            log.LogInformation($"Message body: {message.Body.ToString()}");
            await aggregationService.ProcessUserActivityEventAsync(message, log);
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing message: {ex.Message}");
            throw; // Let Azure Service Bus handle retries
        }
    }
}
