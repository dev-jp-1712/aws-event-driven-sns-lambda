using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AWSEventDrivenDemo.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AWSEventDrivenDemo.Infrastructure.Messaging
{
    /// <summary>
    /// Publishes OrderCreatedEvent to AWS SNS with message attributes for filtering and fan-out.
    /// Multiple Lambda functions can subscribe to the topic and filter events using these attributes.
    /// </summary>
    public class SnsEventPublisher
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly string _topicArn;
        private readonly ILogger<SnsEventPublisher> _logger;

        public SnsEventPublisher(IAmazonSimpleNotificationService snsClient, string topicArn, ILogger<SnsEventPublisher> logger)
        {
            _snsClient = snsClient;
            _topicArn = topicArn;
            _logger = logger;
        }

        /// <summary>
        /// Publishes an OrderCreatedEvent to SNS with message attributes for event filtering.
        /// </summary>
        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent)
        {
            var message = JsonSerializer.Serialize(orderEvent);
            var request = new PublishRequest
            {
                TopicArn = _topicArn,
                Message = message,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "EventType", new MessageAttributeValue { DataType = "String", StringValue = "OrderCreated" } },
                    { "OrderId", new MessageAttributeValue { DataType = "String", StringValue = orderEvent.OrderId.ToString() } },
                    { "CustomerSegment", new MessageAttributeValue { DataType = "String", StringValue = "Retail" } }
                }
            };
            try
            {
                var response = await _snsClient.PublishAsync(request);
                _logger.LogInformation($"Published OrderCreatedEvent to SNS. MessageId: {response.MessageId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing OrderCreatedEvent to SNS");
                throw;
            }
        }
    }
}
