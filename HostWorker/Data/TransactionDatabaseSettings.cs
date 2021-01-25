using System;
using System.Collections.Generic;
using System.Text;

namespace HostWorker.Data
{
    public class TransactionDatabaseSettings : ITransactionDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ITransactionDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
