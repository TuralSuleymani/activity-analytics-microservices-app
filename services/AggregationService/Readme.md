	   # Aggregation Service (Azure Function)

## Overview

The **Aggregation Service** is an Azure Function that processes user activity events from the **Service Bus** and performs data aggregation. It listens to messages from the **user-activity-events** queue in **Azure Service Bus**, aggregates the data (e.g., page views per day, logins by country), and stores the results in **Azure Table Storage**. 

The service is part of an event-driven architecture where the **Tracking Service** emits user activity events, and the **Dashboard Service** fetches aggregated data for display.

## Responsibilities

- **Consume Events**: Listens for user activity events (e.g., page views, logins) from the **Azure Service Bus**.
- **Data Aggregation**: Aggregates events by calculating the number of page views per day and logins by country.
- **Data Storage**: Stores the aggregated data in **Azure Table Storage**.

## Components

1. **Service Bus**: Receives user activity events from the **Tracking Service**.
2. **Azure Function (Aggregation Service)**: Consumes the events and performs data aggregation.
3. **Azure Table Storage**: Stores the aggregated data (e.g., number of page views per day, logins by country).
4. **Azure Key Vault**: Used to securely manage the connection strings for **Azure Service Bus** and **Azure Table Storage**.

## Technologies Used

- **Azure Functions** (Serverless Compute)
- **Azure Service Bus** (Event-driven Communication)
- **Azure Table Storage** (Data Storage for Aggregated Metrics)
- **Azure Key Vault** (Secure Storage for Connection Strings)

## Configuration

The **Aggregation Service** requires configuration values for connecting to **Azure Service Bus** and **Azure Table Storage**. These values are securely stored in **Azure Key Vault** and are accessed by the function using **Managed Identity**.

### App Settings (in Azure Function)

- **KeyVaultUri**: URI of the Azure Key Vault used to retrieve the connection strings.
- **ServiceBusConnectionString**: Connection string for **Azure Service Bus**.
- **TableStorageConnectionString**: Connection string for **Azure Table Storage**.

### Required Azure Key Vault Secrets

- **CosmosDbConnectionString**: Connection string for Cosmos DB (used by Tracking Service).
- **AzureTableStorageConnectionString**: Connection string for Azure Table Storage (used for storing aggregated data).

## Data Flow

1. **Tracking Service** emits user activity events (e.g., page views, logins) to the **user-activity-events** queue in **Azure Service Bus**.
2. The **Aggregation Service** (Azure Function) listens to the **Service Bus** queue.
3. On receiving a message, the function performs the aggregation (e.g., counts page views per day or logins by country).
4. The aggregated data is stored in **Azure Table Storage**.
5. The **Dashboard Service** queries **Azure Table Storage** to fetch aggregated data and display it to the user.

## Why Azure Table Storage?
- **Cost-Effective**: Azure Table Storage is a low-cost, NoSQL data store designed for large-scale applications with high throughput and low latency.
- **Simple Key-Value Store**: Ideal for scenarios where you need to store large amounts of structured data with a simple key-value structure.
- **Scalability**: Azure Table Storage is designed for massive scale, supporting **horizontal scaling** through partitioning, making it suitable for large-scale, distributed applications.
- **Integrated with Azure Ecosystem**: Seamless integration with other **Azure services** such as **Azure Functions**, **Azure Logic Apps**, and **Azure App Service**.
- **Low Latency Reads and Writes**: Azure Table Storage offers low-latency reads and writes, making it suitable for time-sensitive applications that require quick data access.
- **NoSQL Data Model**: It supports a **NoSQL** data model, which is flexible and ideal for applications that deal with semi-structured data.
- **Partitioning and Replication**: Azure Table Storage provides automatic **partitioning** for large datasets, which helps with scalability and availability.
- **RESTful API**: Data can be accessed via **HTTP/HTTPS** using a **RESTful API**, allowing for easy integration into web applications and distributed systems.
- **Availability**: It offers **high availability** with **geo-redundant storage** (GRS) to ensure fault tolerance and disaster recovery.

### Alternatives:
- **Amazon DynamoDB**: A fully managed NoSQL database with fast and predictable performance.
- **Google Bigtable**: A scalable, low-latency NoSQL database that is optimized for high throughput and large-scale applications.
- **Apache Cassandra**: A distributed NoSQL database designed for handling large amounts of data across many commodity servers without a single point of failure.

## Function Triggers

The function is triggered by messages from the **Service Bus** queue. It processes the events asynchronously, ensuring that aggregation happens in a decoupled, scalable manner.

```csharp
[FunctionName("ProcessUserActivityEvent")]
public static async Task RunAggregationJob(
    [ServiceBusTrigger("user-activity-events", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
    ILogger log)
{
    log.LogInformation($"Aggregation job triggered by Service Bus message at: {DateTime.UtcNow}");
    await aggregationService.ProcessUserActivityEventAsync(message, log);
}

