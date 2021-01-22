using System;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareUp.Models;

namespace ShareUp.Services
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

        public async Task<List<Transaction>> GetTransactions(string userid) =>
            await store.Find(t => t.Userid == userid).ToListAsync();

        public async Task<Transaction> Get(string link) =>
            await store.Find(t => t.Link == link).FirstOrDefaultAsync();

        public async Task<Transaction> Create(Transaction model)
        {
            var temp = await store.Find(t => t.Hash == model.Hash)
                    .FirstOrDefaultAsync();
            
            if (temp != null) return null;
            await store.InsertOneAsync(model);
            return model;
        }

        public async Task Cleanup()
        {
            DateTime Now = DateTime.Now;
            var list = await store.Find(t => DateTime.Compare(Now, t.Expires) > 0).ToListAsync();

            foreach(var file in list)
                File.Delete(file.Path);

            var filter = Builders<Transaction>.Filter.Eq(t => DateTime.Compare(Now, t.Expires), 1);
            await store.DeleteManyAsync(filter);
        }
    }
}
