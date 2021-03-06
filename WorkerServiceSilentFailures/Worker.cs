using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static WorkerServiceSilentFailures.ExceptionFilterUtility;

namespace WorkerServiceSilentFailures
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <summary>
        /// This problem will surface as BackgroundService instances just stopping, without any indication of a problem. 
        /// What actually happens if ExecuteAsync throws an exception is that the exception is captured and placed on the Task that was returned from ExecuteAsync. 
        /// The problem is that BackgroundService doesn’t observe that task, so there’s no logging and no process crash - the BackgroundService has completed executing but it just sits there doing nothing. 
        /// </summary>
        /// <inheritdoc cref="https://blog.stephencleary.com/2020/05/backgroundservice-gotcha-silent-failure.html"/>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception exc) when (False(() => _logger.LogCritical(exc, "Fatal error")))
                {
                    throw;
                }
                finally
                {
                    _hostApplicationLifetime.StopApplication();
                }
            }

        }
    }
}
