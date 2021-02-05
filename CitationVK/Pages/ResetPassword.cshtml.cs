using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly Models.Context _context;

        public ResetPasswordModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [DisplayName("Security question")]
        public int Question { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required]
        public string Answer { get; set; }

        public async Task<IActionResult> OnGet(string email)
        {
            if (email == null || HttpContext.Session.GetInt32("id") != null)
            {
                return RedirectToPage("Error");
            }

            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);

            if (account == null)
            {
                return RedirectToPage("Error");
            }

            Email = account.Email;
            Question = account.Question;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Email);

            Answer = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Answer,
                salt: Convert.FromBase64String(account.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            if (account.Answer != Answer)
            {
                ModelState.AddModelError("Answer", "The provided security question answer is incorrect.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(account).State = EntityState.Modified;

            account.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: Convert.FromBase64String(account.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            await _context.SaveChangesAsync();
            return RedirectToPage("Login");
        }
    }
}
