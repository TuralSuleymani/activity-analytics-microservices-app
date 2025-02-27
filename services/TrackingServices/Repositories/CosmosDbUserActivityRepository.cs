using Microsoft.Azure.Cosmos;

using TrackingServices.Models;

namespace TrackingServices.Repositories
{
    public class CosmosDbUserActivityRepository : IUserActivityRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosDbUserActivityRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            var database = _cosmosClient.GetDatabase("UserActivityTracking");
            _container = database.GetContainer("ActivityEvents");
        }

        public async Task StoreActivityAsync(UserActivityEvent activityEvent)
        {
            await _container.CreateItemAsync(activityEvent, new PartitionKey(activityEvent.TenantId));
        }
    }
}
