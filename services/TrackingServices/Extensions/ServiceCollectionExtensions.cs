using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Cosmos;

namespace TrackingService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            var client = GetKeyValueClient(configuration);

            var cosmosDbUri = client.GetSecret("CosmosDbUr").Value.Value;
            var cosmosDbPrimaryKey = client.GetSecret("CosmosDbPrimaryKey").Value.Value;

            return services.AddSingleton(serviceProvider =>
             {
                 return new CosmosClient(cosmosDbUri, cosmosDbPrimaryKey);
             });
        }

        private static SecretClient GetKeyValueClient(IConfiguration configuration)
        {
            var keyVaultUri = configuration["KeyVault:VaultUri"];
            return new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

        }
        public static IServiceCollection AddServiceBusClient(this IServiceCollection services, IConfiguration configuration)
        {
            var client = GetKeyValueClient(configuration);

            var serviceBusConnectionString = client.GetSecret("ServiceBusConnectionString").Value.Value;
            return services.AddSingleton<ServiceBusClient>(serviceProvider =>
             {
                 return new ServiceBusClient(serviceBusConnectionString);
             });
        }
    }
}
