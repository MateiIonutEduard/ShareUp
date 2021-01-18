using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ShareUp.Services;

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class RequestModel : PageModel
    {
        private readonly ILogger<RequestModel> _logger;
        protected readonly TransactionService trans;

        public RequestModel(ILogger<RequestModel> logger, TransactionService trans)
        {
            _logger = logger;
            this.trans = trans;
        }

        public void OnGet()
        { }

        public async Task<FileResult> OnGetDownloadFile(string link)
        {
            var item = await trans.Get(link);
            int last = item.Path.LastIndexOf('/');
            string name = item.Path.Substring(last + 1);
            byte[] buffer = System.IO.File.ReadAllBytes(item.Path);
            return File(buffer, "application/octet-stream", name);
        }
    }
}
