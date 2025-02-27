namespace TrackingServices.Models
{
    public class UserActivityEvent
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string TenantId { get; set; } 
        public string UserId { get; set; } 
        public string PageUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }  // Type of event (e.g., "PageView", "Login")
        public string Country { get; set; } 
        public bool IsFirstTimeLogin { get; set; } 
    }
}
