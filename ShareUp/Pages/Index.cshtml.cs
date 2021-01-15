using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ShareUp.Models;
using ShareUp.Services;
using System.Text;

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TransactionService ts;
        private Random rand;

        public IndexModel(ILogger<IndexModel> logger, TransactionService ts)
        {
            rand = new Random(Environment.TickCount);
            _logger = logger;
            this.ts = ts;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string from, string[] to, IFormFile[] files)
        {
            string table = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuWwXxYyZz0123456789";
            byte[] link = new byte[16];

            for (int i = 0; i < link.Length; i++)
                link[i] = (byte)table[rand.Next(table.Length)];

            string code = Encoding.UTF8.GetString(link);
            var transaction = new Transaction
            {
                From = from,
                Path = $"./Storage/{code}.zip",
                To = to.ToList(),
                Link = code,
                Expires = DateTime.Now.AddDays(15)
            };

            await ts.Create(transaction);
            // Save files as archive on disk...
            return Page();
        }

        public async Task<IActionResult> OnDeleteAsync()
        {
            await ts.Cleanup();
            return Page();
        }
    }
}
