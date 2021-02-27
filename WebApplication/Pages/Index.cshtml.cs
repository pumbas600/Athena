using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string Name => (string) TempData[nameof(Name)];
        public string Username => (string) TempData[nameof(Username)];

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostLogin([FromForm] string username, [FromForm] string password)
        {
            TempData["Username"] = username + ":" + password;
            return RedirectToPage();
        }
    }
}