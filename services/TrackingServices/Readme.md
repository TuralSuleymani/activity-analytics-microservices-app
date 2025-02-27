# TrackingService

## Purpose

The **TrackingService** is a backend microservice designed to track user activity within a multi-tenant web application. It collects data such as page views, user logins, and other interactions. The service stores this data in **Azure Cosmos DB** for scalability and fast access, and emits events to an **Azure Service Bus** to notify other services (e.g., Aggregation Service) for processing.

### Key Features:
- **Tracking User Activity**: Collects user activity data like page views and logins.
- **Event Emission**: Emits events to a **Service Bus** for further processing by the Aggregation Service.
- **Scalability**: Uses **Azure Cosmos DB** for storing raw activity data to handle high throughput.
- **Secure Access**: Secures the API with **JWT** authentication to ensure only authorized users can track activities.
  
## Used Packages

- **Microsoft.Azure.Cosmos**
- **Azure.Messaging.ServiceBus**
- **Microsoft.AspNetCore.Authentication.JwtBearer**
- **Azure.Identity**
- **Azure.Security.KeyVault.Secrets**
- **Newtonsoft.Json**

---

## Project Structure

The **TrackingService** solution consists of the following components:

- **API (TrackingController)**: Provides endpoints to track user activity (e.g., page views, logins).
  - Endpoint: `POST /api/tracking/track` - Tracks a user activity event.
  
- **Repositories**: Handles communication with **Azure Cosmos DB** to store activity data.
  - `CosmosUserActivityRepository`: Implements logic to interact with **Cosmos DB**.
  
- **Event Emission**: Emits events to **Azure Service Bus** for the Aggregation Service to consume and process.

- **Security**: Implements **JWT Authentication** to ensure that only authorized users can access the tracking data.

## Why Azure Cosmos DB?
- high-volume, low-latency writes
- built-in global distribution 
- Supports document (JSON), graph, key-value, and column-family data models
- Guarantees millisecond read and write latencies 
- SQL-like querying capabilities
- supports horizontal scaling with partition keys, which can align with your multi-tenant architecture
- Alternatives : Apache Cassandra, Amazon Dynamo DB, Google Bigtable: