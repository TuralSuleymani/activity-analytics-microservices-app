using TrackingServices.Models;

namespace TrackingServices.Repositories
{
    public interface IUserActivityRepository
    {
        Task StoreActivityAsync(UserActivityEvent activityEvent);
    }
}
