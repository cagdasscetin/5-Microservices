using MassTransit;
using MediatR;
using Micro.Application.Interfaces.Repositories;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Quartz;

namespace OutboxWorker.Jobs;

[DisallowConcurrentExecution]
public class OutboxJob : IJob
{
    private static int _attemptCount = 0;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPolicyRegistry<string> _policyRegistry;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly IAsyncPolicy _retryPolicy;

    public OutboxJob(IOutboxRepository outboxRepository, IPublishEndpoint publishEndpoint, IPolicyRegistry<string> policyRegistry)
    {
        _outboxRepository = outboxRepository;
        _publishEndpoint = publishEndpoint;
        _policyRegistry = policyRegistry;
        _circuitBreakerPolicy = _policyRegistry.Get<IAsyncPolicy>("CircuitBreakerPolicy");
        _retryPolicy = _policyRegistry.Get<IAsyncPolicy>("RetryPolicy");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var outboxMessages = await _outboxRepository.GetAllAsync(m => !m.IsSent);

        var messages = outboxMessages.Take(10).ToList();
        
        foreach (var message in messages)
        {
            var eventType = Type.GetType(message.EventType + ", Micro.Application");
            var @event = JsonConvert.DeserializeObject(message.EventPayload, eventType);

            if (@event is null)
                continue;
            
            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _attemptCount++;
                    await _publishEndpoint.Publish(@event, context.CancellationToken).ConfigureAwait(false);

                    // throw new Exception($"deneme {_attemptCount}");

                    message.SentDate = DateTime.UtcNow;
                    message.IsSent = true;

                    await _outboxRepository.UpdateAsync(message);
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine($"Attempt {_attemptCount}");
                Console.WriteLine("Circuit is broken! Skipping operation.");
            }
            catch (Exception ex)
            {
                var asd = _attemptCount;
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        await _outboxRepository.SaveChangesAsync();
    }
}