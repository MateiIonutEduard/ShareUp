using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class RequestModel : PageModel
    {
        private readonly ILogger<RequestModel> _logger;

        public RequestModel(ILogger<RequestModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        { }
    }
}
