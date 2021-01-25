using System;
using System.IO;
using System.Net;
using System.Net.Mail;
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
using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
#pragma warning disable

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly AdminService ads;
        private readonly ILogger<IndexModel> _logger;
        private readonly TransactionService ts;
        private readonly AccountService account;
        private Random rand;

        public IndexModel(AccountService account, ILogger<IndexModel> logger, AdminService ads, TransactionService ts)
        {
            this.account = account;
            rand = new Random(Environment.TickCount);
            _logger = logger;
            this.ads = ads;
            this.ts = ts;
        }

        public void OnGet()
        { }

        [Authorize]
        public async Task<IActionResult> OnPostAsync(string[] to, IFormFile[] files)
        {
            string table = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuWwXxYyZz0123456789";
            byte[] link = new byte[16];

            for (int i = 0; i < link.Length; i++)
                link[i] = (byte)table[rand.Next(table.Length)];

            var userid = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "userid")?.Value;

            var from = HttpContext.User?.Claims?
                .FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(userid))
            {
                string code = Encoding.UTF8.GetString(link);
                Directory.CreateDirectory($"./Storage/{code}");

                foreach (var file in files)
                {
                    var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    var path = file.FileName;
                    System.IO.File.WriteAllBytes($"./Storage/{code}/{path}", ms.ToArray());
                }

                ZipFile.CreateFromDirectory($"./Storage/{code}", $"./Storage/{code}.zip");

                var fs = new FileStream($"./Storage/{code}.zip", FileMode.Open);
                var run = MD5.Create();

                byte[] buffer = run.ComputeHash(fs);
                string hash = Convert.ToBase64String(buffer);
                fs.Close();

                var transaction = new Transaction
                {
                    Userid = userid,
                    Path = $"./Storage/{code}.zip",
                    To = to.ToList(),
                    Hash = hash,
                    Link = code,
                    Expires = DateTime.Now.AddDays(15)
                };

                await ts.Create(transaction);
                Directory.Delete($"./Storage/{code}", true);

                foreach (var address in to)
                {
                    string content = $"Hi there!<br>You received an attachment from <b style='color: #5f9ea0;'>{from}</b>.<br> link: <a href='{account.app.domain}Request/?token={code}'>{account.app.domain}/Request/?token={code}</a><br><br>Have a nice day!";
                    ads.SendEmail(address, "ShareUp", content);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnDeleteAsync()
        {
            await ts.Cleanup();
            return new OkResult();
        }
    }
}
