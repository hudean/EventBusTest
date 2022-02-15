using EventBus.Abstractions;
using EventBusSubscribeTest.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBusSubscribeTest.IntegrationEvents.EventHandling
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(
           //IBasketRepository repository,
           ILogger<OrderStartedIntegrationEventHandler> logger)
        {
           // _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            using (Serilog.Context.LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                // await _repository.DeleteBasketAsync(@event.UserId.ToString());
            }
            Console.WriteLine("订阅执行");
            return Task.CompletedTask;
        }
    }
}
