using System;
using ShareUp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace ShareUp.Services
{
    public class AccountService
    {
        public readonly IAppSettings app;
        private readonly IMongoCollection<User> users;

        public AccountService(IAppSettings app, ITransactionDatabaseSettings settings)
        {
            this.app = app;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            users = database.GetCollection<User>("Users");
        }

        public async Task<(string, string)> Login(string address, string password)
        {
            string key = Encrypt(password);
            var user = await users.Find(u => u.Address == address && u.Password == key).FirstOrDefaultAsync();

            if (user != null) return (user.Username, user.Id);
            else
            {
                user = await users.Find(u => u.Address == address || u.Password == key).FirstOrDefaultAsync();
                if (user == null) return (null, null);
                else return (string.Empty, string.Empty);
            }
        }

        public async Task<(string, string)> Signup(string username, string password, string address)
        {
            string passkey = Encrypt(password);
            var exists = await users.Find(u => u.Address == address && u.Username == username)
                    .FirstOrDefaultAsync();

            if (exists != null) return (null, null);
            else
            {
                var user = new User
                {
                    Username = username,
                    Password = passkey,
                    Address = address
                };

                await users.InsertOneAsync(user);
                return (user.Username, user.Id);
            }
        }

        public async Task<(string, string)> FindPassword(string address)
        {
            var user = await users.Find(u => u.Address == address)
                .FirstOrDefaultAsync();

            if (user != null) return (user.Username, Decrypt(user.Password));
            else return (string.Empty, string.Empty);
        }

        public string Encrypt(string val)
        {
            byte[] key = Convert.FromBase64String(app.key);
            byte[] salt = Convert.FromBase64String(app.salt);

            AesManaged model = new AesManaged();
            var crypt = model.CreateEncryptor(key, salt);
            MemoryStream ms = new MemoryStream();
            
            using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                    sw.Write(val);

                byte[] buffer = ms.ToArray();
                string encrypted = Convert.ToBase64String(buffer);
                return encrypted;
            }
        }

        public string Decrypt(string val)
        {
            byte[] key = Convert.FromBase64String(app.key);
            byte[] salt = Convert.FromBase64String(app.salt);
            byte[] encrypted = Convert.FromBase64String(val);

            AesManaged model = new AesManaged();
            var crypt = model.CreateDecryptor(key, salt);
            MemoryStream ms = new MemoryStream(encrypted);

            using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
            {
                string plain = string.Empty;

                using (StreamReader sr = new StreamReader(cs))
                    plain = sr.ReadToEnd();

                return plain;
            }
        }
    }
}
