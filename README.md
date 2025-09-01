# AWSEventDrivenDemo

## Event-Driven Architecture with AWS Lambda and SNS (.NET 9)

This repository demonstrates a modern event-driven architecture using .NET 9, AWS Lambda, and Amazon SNS.  
It shows how to decouple services, scale automatically, and build resilient systems using serverless patterns.

---

## Solution Overview

**Projects:**
- **AWSEventDrivenDemo.Domain**: Shared domain models and events.
- **AWSEventDrivenDemo.Infrastructure**: SNS publisher logic.
- **AWSEventDrivenDemo.Api**: REST API for order creation.
- **AWSEventDrivenDemo.LambdaHandler**: AWS Lambda handler for event processing.

---

## Architecture

<img width="1000" height="667" alt="Untitled diagram _ Mermaid Chart-2025-09-01-110804" src="https://github.com/user-attachments/assets/c4da986c-0056-470c-8555-9947045cbfbc" />

- The API publishes events to SNS.
- SNS fans out events to multiple Lambda subscribers.
- Failed Lambda executions are sent to an SQS Dead Letter Queue (DLQ).

---

## Features

- **Loose Coupling**: Decoupled services via events.
- **Scalability**: Add new consumers without changing producers.
- **Idempotency**: Prevent duplicate event processing.
- **Fan-Out**: SNS delivers events to multiple subscribers.
- **DLQ & Retry**: Failed events are retried and sent to DLQ for analysis.
- **Observability**: Integrated with CloudWatch Logs and AWS X-Ray.

---

## Getting Started

### Prerequisites

- AWS Account
- .NET 9 SDK
- AWS CLI (configured)
- AWS Toolkit for Visual Studio (optional)

### Project Setup

```sh
# Clone the repo
git clone https://github.com/<your-username>/AWSEventDrivenDemo.git
cd AWSEventDrivenDemo

# Restore and build
dotnet restore
dotnet build
```

### Solution Structure

```
AWSEventDrivenDemo/
├── AWSEventDrivenDemo.Domain/
├── AWSEventDrivenDemo.Infrastructure/
├── AWSEventDrivenDemo.Api/
├── AWSEventDrivenDemo.LambdaHandler/
```

---

## How It Works

1. **Order API** receives a new order and publishes an `OrderCreatedEvent` to SNS.
2. **SNS** fans out the event to multiple Lambda functions (OrderProcessor, Audit, Notification).
3. **Lambda Handlers** process the event independently.
4. **DLQ** captures failed events for later analysis.

---

## Key Code Sections

### Domain Model

```csharp
public class Order { ... }
public class OrderItem { ... }
public class OrderCreatedEvent { ... }
```

### SNS Publisher

```csharp
public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent)
{
    // ... see AWSEventDrivenDemo.Infrastructure/SnsEventPublisher.cs ...
}
```

### API Endpoint

```csharp
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
{
    // ... see AWSEventDrivenDemo.Api/Controllers/OrdersController.cs ...
}
```

### Lambda Handler

```csharp
public async Task Handler(SNSEvent snsEvent, ILambdaContext context)
{
    // ... see AWSEventDrivenDemo.LambdaHandler/OrderCreatedLambdaHandler.cs ...
}
```

---

## AWS Setup

- **SNS Topic**: Create in AWS Console.
- **Lambda Functions**: Create and subscribe to SNS topic.
- **DLQ**: Configure SQS queue as DLQ for Lambda.
- **IAM Roles**: Ensure Lambda has permissions for SNS and SQS.

---

## Observability

- **CloudWatch Logs**: All Lambda logs are sent to CloudWatch.
- **AWS X-Ray**: Enable for distributed tracing.

---

## Deployment

- Use AWS Toolkit for Visual Studio or AWS CLI for deployment.
- Store environment-specific config in Lambda environment variables.

---

## Common Errors & Solutions

- **IAM Permissions**: Attach `AmazonSNSFullAccess` and `AmazonSQSFullAccess` to Lambda role.
- **Timeouts**: Increase Lambda timeout if needed.
- **Serialization Issues**: Ensure event schema matches model.
- **DLQ Not Configured**: Always configure DLQ for async Lambda triggers.

---

## References

- [AWS Lambda + SNS Documentation](https://docs.aws.amazon.com/lambda/latest/dg/with-sns.html)
- [Event-driven architecture using AWS S3, Lambda, and SNS services](https://pappusanodiya.medium.com/event-driven-architecture-using-aws-s3-lambda-and-sns-services-d4c8ca9d1eb)
- [Leverage AWS S3, Lambda, SNS to setup event-driven messaging service](https://www.cloudthat.com/resources/blog/leverage-aws-s3-lambda-sns-to-setup-event-driven-messaging-service)

---

## License

MIT

---

**For full implementation details, see individual project folders and code files.  
For a step-by-step guide and architecture explanation, read the [Medium blog](<your-blog-link>).**
