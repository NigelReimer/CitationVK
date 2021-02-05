using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class ShareClassifierModel : PageModel
    {
        private readonly Models.Context _context;

        public ShareClassifierModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [DisplayName("Classifier")]
        public int ClassifierId { get; set; }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public List<Models.Classifier> Classifiers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.AccountClassifiers.FirstOrDefaultAsync(x => x.ClassifierId == ClassifierId && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            Models.Classifier classifier = await _context.Classifiers.FirstOrDefaultAsync(x => x.Id == ClassifierId);
            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Email && !x.IsAdmin);

            if (account == null)
            {
                ModelState.AddModelError("Email", "No account with this email address exists.");
            }
            else if (await _context.Classifiers.FirstOrDefaultAsync(x => x.Name == classifier.Name && (x.AccountClassifiers.FirstOrDefault(y => y.AccountId == account.Id) != null)) != null)
            {
                ModelState.AddModelError("ClassifierId", "User already has a classifier with this name.");
            }

            if (!ModelState.IsValid)
            {
                Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            _context.AccountClassifiers.Add(new Models.AccountClassifier() { Account = account, Classifier = classifier });
            await _context.SaveChangesAsync();
            return RedirectToPage("Classifiers");
        }
    }
}
