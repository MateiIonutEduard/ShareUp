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

        public async Task<Transaction> GetSigle(string id) =>
            await store.Find(t => t.Id == id).FirstOrDefaultAsync();

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

        public async Task RemoveItem(Transaction trans)
        {
            await store.DeleteOneAsync(t => t.Id == trans.Id);
        }
    }
}
