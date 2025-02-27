using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using TrackingServices.Models;
using TrackingServices.Repositories;

namespace TrackingService.Services
{
    public class TrackingService(IUserActivityRepository userActivityRepository
            , ServiceBusClient serviceBusClient) : ITrackingService
    {
        private readonly IUserActivityRepository _repository = userActivityRepository;
        private readonly ServiceBusClient _serviceBusClient = serviceBusClient;
        private readonly string _queueName = "user-activity-events";

        public async Task TrackActivityAsync(UserActivityEvent activityEvent)
        {
            await _repository.StoreActivityAsync(activityEvent);

            await EmitEventToServiceBusAsync(activityEvent);
        }

        private async Task EmitEventToServiceBusAsync(UserActivityEvent activityEvent)
        {
            var sender = _serviceBusClient.CreateSender(_queueName);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(activityEvent));
            await sender.SendMessageAsync(message);
        }
    }
}
