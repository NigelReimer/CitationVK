using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class CreateAccountModel : PageModel
    {
        private readonly Models.Context _context;

        public CreateAccountModel(Models.Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.Get("isAdmin") != null && !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                return RedirectToPage("Error");
            }

            Configuration = await _context.Configurations.FirstOrDefaultAsync(x => x.Id == 1);
            return Page();
        }

        [BindProperty]
        public Models.Account Account { get; set; }

        public Models.Configuration Configuration { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Data.Utilities.Questions.ContainsKey(Account.Question))
            {
                return RedirectToPage("Error");
            }

            if (await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Account.Email) != null)
            {
                ModelState.AddModelError("Account.Email", "An account with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                Configuration = await _context.Configurations.FirstOrDefaultAsync(x => x.Id == 1);
                return Page();
            }

            byte[] salt = new byte[128 / 8];
            
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            byte[] password = KeyDerivation.Pbkdf2(
                password: Account.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );

            byte[] answer = KeyDerivation.Pbkdf2(
                password: Account.Answer,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );

            if (HttpContext.Session.Get("isAdmin") == null || !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                Account.IsAdmin = false;
            }

            Models.Account account = new Models.Account()
            {
                Email = Account.Email,
                Password = Convert.ToBase64String(password),
                Question = Account.Question,
                Answer = Convert.ToBase64String(answer),
                Salt = Convert.ToBase64String(salt),
                IsAdmin = Account.IsAdmin,
                Date = DateTime.Now
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            if (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                return RedirectToPage("Accounts");
            }

            HttpContext.Session.SetInt32("id", account.Id);
            HttpContext.Session.Set("isAdmin", BitConverter.GetBytes(account.IsAdmin));
            return RedirectToPage("Index");
        }
    }
}
