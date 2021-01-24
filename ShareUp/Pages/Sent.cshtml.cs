using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShareUp.Services;
using ShareUp.Models;
using Microsoft.AspNetCore.Authorization;
#pragma warning disable

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class SentModel : PageModel
    {
        private readonly TransactionService ts;
        private readonly AccountService account;

        public SentModel(AccountService account, TransactionService ts)
        {
            this.account = account;
            this.ts = ts;
        }

        public void OnGet()
        { }

        [Authorize]
        public async Task<IActionResult> OnDeleteAsync(string id)
        {
            var userid = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "userid")?.Value;

            var t = await ts.GetSigle(id);

            if (t.Userid == userid)
            {
                System.IO.File.Delete(t.Path);
                await ts.RemoveItem(t);
                return new OkResult();
            }

            return Unauthorized();
        }
    }
}
