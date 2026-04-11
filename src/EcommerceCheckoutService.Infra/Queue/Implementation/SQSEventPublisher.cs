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
        ArgumentException.ThrowIfNullOrEmpty(queue, nameof(queue));

        AmazonSQSClient client = new (Amazon.RegionEndpoint.SAEast1);
        GetQueueUrlResponse queueUrl;

        try
        {
            queueUrl = await client.GetQueueUrlAsync(queue);
        }
        catch (QueueDoesNotExistException ex)
        {
            throw new InvalidOperationException($"Queue '{queue}' does not exist.", ex);
        }

        if (queueUrl is null || string.IsNullOrEmpty(queueUrl.QueueUrl))
            throw new InvalidOperationException($"Queue URL for '{queue}' was not returned.");

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