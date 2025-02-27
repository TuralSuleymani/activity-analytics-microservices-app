# API Gateway Configuration with Ocelot

## Tracking Service
- **Load Balancing**: Implemented using the `RoundRobin` strategy for multiple instances (`5001`, `5002`).
- **Authentication**: Secured with JWT-based authentication.
- **Rate Limiting**: Configured to prevent abuse, allowing up to 100 requests per minute per client.
- **Response Caching**: Optimized for performance with a Time-to-Live (TTL) of 300 seconds.

## Dashboard(Reporting) Service
- **Direct Connection**: Routes directly to a single instance (`5003`).
- **Rate Limiting**: Configured to allow up to 200 requests per minute per client.
- **Response Caching**: Enabled with a TTL of 300 seconds to improve performance.

## Global Configuration
- **Shared Caching**: Unified caching configuration across all routes.
- **Rate-Limiting Headers**: Included in responses for enhanced visibility and user feedback.
