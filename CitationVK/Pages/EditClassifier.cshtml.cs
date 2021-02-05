using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class EditClassifierModel : PageModel
    {
        private readonly Models.Context _context;

        public EditClassifierModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        public Models.Classifier Classifier { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            if (id == null)
            {
                return RedirectToPage("Error");
            }

            Classifier = await _context.Classifiers.FirstOrDefaultAsync(x => x.Id == id);

            if (Classifier == null || await _context.AccountClassifiers.FirstOrDefaultAsync(x => x.ClassifierId == Classifier.Id && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            Name = Classifier.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Classifiers");
            }

            Models.Classifier classifier = await _context.Classifiers.FirstOrDefaultAsync(x => x.Id == id);

            if (await _context.Classifiers.FirstOrDefaultAsync(x => x.Name == Name && (x.AccountClassifiers.FirstOrDefault(y => y.AccountId == HttpContext.Session.GetInt32("id")) != null)) != null)
            {
                ModelState.AddModelError("Name", "You already have a classifier with this name.");
                Classifier = await _context.Classifiers.FirstOrDefaultAsync(x => x.Id == id);
                Name = Classifier.Name;
                return Page();
            }

            if (classifier != null && await _context.AccountClassifiers.FirstOrDefaultAsync(x => x.ClassifierId == classifier.Id && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                _context.Attach(classifier).State = EntityState.Modified;
                classifier.Name = Name;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Classifiers");
        }
    }
}
