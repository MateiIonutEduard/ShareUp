using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HostWorker.Data;

namespace HostWorker
{
    public class Worker : BackgroundService
    {
        private readonly TransactionService trans;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, TransactionService trans)
        {
            this.trans = trans;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await trans.Cleanup();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
