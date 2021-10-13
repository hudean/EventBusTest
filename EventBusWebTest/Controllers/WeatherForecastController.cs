using EventBus.Abstractions;
using EventBusWebTest.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBusWebTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IEventBus _eventBus;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
             IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public  IActionResult PublishEventMessage()
        {
            string userid = Guid.NewGuid().ToString();
            var eventMessage = new OrderStartedIntegrationEvent(userid);
            try
            {
                _eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

                throw;
            }

            return Accepted();
        }

        //[HttpGet]
        //public async Task<IActionResult> PublishEventMessage()
        //{
        //    string userid = Guid.NewGuid().ToString();
        //    var eventMessage = new OrderStartedIntegrationEvent(userid);
        //    try
        //    {
        //        _eventBus.Publish(eventMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

        //        throw;
        //    }

        //    return Accepted();
        //}
    }
}
