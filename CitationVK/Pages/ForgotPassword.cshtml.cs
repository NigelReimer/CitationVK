using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly Models.Context _context;

        public ForgotPasswordModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                return RedirectToPage("Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Email);

            if (account == null)
            {
                ModelState.AddModelError("Email", "No account with this email address found.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("ResetPassword", new { email = Email });
        }
    }
}
