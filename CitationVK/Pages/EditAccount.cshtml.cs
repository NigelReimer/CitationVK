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
    public class EditAccountModel : PageModel
    {
        private readonly Models.Context _context;

        public EditAccountModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Account Account { get; set; }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string Answer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || HttpContext.Session.GetInt32("id") == null || HttpContext.Session.Get("isAdmin") == null)
            {
                return RedirectToPage("Error");
            }
            else if (HttpContext.Session.GetInt32("id") != id && !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                return RedirectToPage("Error");
            }

            Account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (Account == null)
            {
                return RedirectToPage("Error");
            }

            Email = Account.Email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.Get("isAdmin") != null && !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                if (HttpContext.Session.GetInt32("id") != null && HttpContext.Session.GetInt32("id") != Account.Id)
                {
                    return RedirectToPage("Error");
                }
            }

            if (await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Email && x.Id != Account.Id) != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Account).State = EntityState.Modified;
            _context.Entry(Account).Property(x => x.Salt).IsModified = false;
            _context.Entry(Account).Property(x => x.Date).IsModified = false;
            Account.Email = Email;

            if (!string.IsNullOrEmpty(Password))
            {
                Account.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Password,
                    salt: Convert.FromBase64String(Account.Salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));
            }
            else
            {
                _context.Entry(Account).Property(x => x.Password).IsModified = false;
            }

            if (!string.IsNullOrEmpty(Answer))
            {
                Account.Answer = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Answer,
                    salt: Convert.FromBase64String(Account.Salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));
            }
            else
            {
                _context.Entry(Account).Property(x => x.Answer).IsModified = false;
            }

            if (!BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                Account.IsAdmin = false;
            }
            else if (Account.Id == HttpContext.Session.GetInt32("id"))
            {
                Account.IsAdmin = true;
            }

            await _context.SaveChangesAsync();

            if (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                return RedirectToPage("Accounts");
            }

            return RedirectToPage("Index");
        }
    }
}
