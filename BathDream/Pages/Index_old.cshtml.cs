using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BathDream.Pages
{
    public class IndexOldModel : PageModel
    {
        private readonly ILogger<IndexOldModel> _logger;

        public IndexOldModel(ILogger<IndexOldModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
