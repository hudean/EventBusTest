using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.EventBusRabbitMQ;
using EventBusSubscribeTest.IntegrationEvents.EventHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBusSubscribeTest
{
    /// <summary>
    /// ServiceProvider的扩展方法
    /// </summary>
    public static class ServiceProviderExtension
    {

        /// <summary>
        /// 注册集成服务（AzureServiceBus或者RabbitMQ集成服务）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region 注册事件总线日志

            //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            //    sp => (DbConnection c) => new IntegrationEventLogService(c));

            //services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            #endregion

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        /// <summary>
        /// 注册事件总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddSingleton<IEventBus,EventBusRabbitMQ>(sp =>
                {
                    var subscriptionClientName = configuration["SubscriptionClientName"];
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new EventBusRabbitMQ(sp, rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });
          

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //注册要订阅事件的处理器
            //services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            services.AddTransient<OrderStartedIntegrationEventHandler>();

            return services;
        }


        /// <summary>
        /// 注册健康检测服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            //hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            //hcBuilder
            //    .AddRedis(
            //        configuration["ConnectionString"],
            //        name: "redis-check",
            //        tags: new string[] { "redis" });

            //if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            //{
            //    hcBuilder
            //        .AddAzureServiceBusTopic(
            //            configuration["EventBusConnection"],
            //            topicName: "eshop_event_bus",
            //            name: "basket-servicebus-check",
            //            tags: new string[] { "servicebus" });
            //}
            //else
            //{
            //    hcBuilder
            //        .AddRabbitMQ(
            //            $"amqp://{configuration["EventBusConnection"]}",
            //            name: "basket-rabbitmqbus-check",
            //            tags: new string[] { "rabbitmqbus" });
            //}

            return services;
        }

     
    }
}
