using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.Core;
using System.Text.Json;
using AWSEventDrivenDemo.Domain.Events;

namespace AWSEventDrivenDemo.LambdaHandler
{
    /// <summary>
    /// Sample AWS Lambda handler for processing OrderCreatedEvent from SNS.
    /// Demonstrates idempotency, fan-out, and message attribute filtering.
    /// </summary>
    public class OrderCreatedLambdaHandler
    {
        // Simulated idempotency store (replace with DynamoDB, Redis, etc. in production)
        private static readonly HashSet<Guid> ProcessedOrderIds = new();

        public async Task Handler(SNSEvent snsEvent, ILambdaContext context)
        {
            foreach (var record in snsEvent.Records)
            {
                // Deserialize event
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(record.Sns.Message);
                if (orderEvent == null) continue;

                // Idempotency check
                if (ProcessedOrderIds.Contains(orderEvent.OrderId))
                {
                    context.Logger.LogLine($"Order {orderEvent.OrderId} already processed. Skipping.");
                    continue;
                }

                // Example: Filter by message attribute (EventType)
                if (record.Sns.MessageAttributes.TryGetValue("EventType", out var eventTypeAttr) && eventTypeAttr.Value != "OrderCreated")
                {
                    context.Logger.LogLine($"Ignoring event type: {eventTypeAttr.Value}");
                    continue;
                }

                // Process order (fan-out: this Lambda could send email, another could update inventory, etc.)
                context.Logger.LogLine($"Processing order {orderEvent.OrderId} for customer {orderEvent.CustomerName}");
                // ... business logic here ...

                // Mark as processed (idempotency)
                ProcessedOrderIds.Add(orderEvent.OrderId);
            }
        }
    }
}
