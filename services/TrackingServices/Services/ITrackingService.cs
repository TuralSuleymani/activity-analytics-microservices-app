using TrackingServices.Models;

namespace TrackingService.Services
{
    public interface ITrackingService
    {
        Task TrackActivityAsync(UserActivityEvent activityEvent);
    }
}