using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostWorker.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
namespace HostWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<TransactionDatabaseSettings>(
                        hostContext.Configuration.GetSection(nameof(TransactionDatabaseSettings)));

                    services.AddSingleton<ITransactionDatabaseSettings>(sp =>
                        sp.GetRequiredService<IOptions<TransactionDatabaseSettings>>().Value);

                    services.AddSingleton<TransactionService>();
                    services.AddHostedService<Worker>();
                });
    }
}
