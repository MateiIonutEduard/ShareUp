using System;
using System.IO;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using HostWorker.Data;

namespace HostWorker.Data
{
    public class TransactionService
    {
        private readonly IMongoCollection<Transaction> store;

        public TransactionService(ITransactionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            store = database.GetCollection<Transaction>("Transactions");
        }

        public async Task Cleanup()
        {
            DateTime Now = DateTime.Now;
            var list = await store.Find(t => true).ToListAsync();

            foreach (var item in list)
            {
                bool done = DateTime.Compare(Now, item.Expires) >= 0;
                if (done)
                {
                    await store.DeleteOneAsync(item => done);

                    foreach (var file in list)
                    {
                        string rel = file.Path.Substring(1);
                        string path = $"../ShareUp/{rel}";
                        File.Delete(path);
                    }
                }
            }
        }
    }
}
