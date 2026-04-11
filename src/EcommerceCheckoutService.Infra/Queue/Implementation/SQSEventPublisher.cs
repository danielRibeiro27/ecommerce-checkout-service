using System.Text.Json;
using Amazon.Runtime.Internal.Util;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace EcommerceCheckoutService.Infra.Queue.Implementation;

public class SQSEventPublisher : IEventPublisher
{
    public async Task PublishAsync<T>(string queue, string topic, T message)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic, nameof(topic));
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        ArgumentNullException.ThrowIfNull(queue, nameof(queue));

        AmazonSQSClient client = new (Amazon.RegionEndpoint.SAEast1);
        var queueUrl = await client.GetQueueUrlAsync(queue);

        if (queueUrl is null || string.IsNullOrEmpty(queueUrl.QueueUrl))
            throw new NullReferenceException($"Queue URL for '{queue}' not found.");

        //Logger.LogInformation($"QUEUE_URL: {queueUrl.QueueUrl}");
        
        string messageBody = JsonSerializer.Serialize(message);
        var sendMessageRequest = new SendMessageRequest()
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = messageBody
        };

        var response = await client.SendMessageAsync(sendMessageRequest);
        //Logger.LogInformation($"MESSAGE_SENT: {response.MessageId}");
    }
}