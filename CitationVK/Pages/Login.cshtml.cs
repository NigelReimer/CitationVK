using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Models.Context _context;

        public LoginModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

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

            if (account != null)
            {
                string password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Password,
                    salt: Convert.FromBase64String(account.Salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));

                if (account.Password == password)
                {
                    HttpContext.Session.SetInt32("id", account.Id);
                    HttpContext.Session.Set("isAdmin", BitConverter.GetBytes(account.IsAdmin));
                    return RedirectToPage("Index");
                }
            }

            ModelState.AddModelError("Password", "Invalid credentials, unable to log in.");
            return Page();
        }
    }
}
