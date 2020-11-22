using System;
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
            store = database.GetCollection<Transaction>(settings.TransactionCollectionName);
        }

        public async Task<Transaction> Get(string link) =>
            await store.Find(t => t.Link == link).FirstOrDefaultAsync();

        public async Task<Transaction> Create(Transaction model)
        {
            await store.InsertOneAsync(model);
            return model;
        }
    }
}
