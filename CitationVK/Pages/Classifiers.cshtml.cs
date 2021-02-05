using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class ClassifiersModel : PageModel
    {
        private readonly Models.Context _context;
        private readonly IHostingEnvironment _environment;

        public ClassifiersModel(Models.Context context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IList<Models.Classifier> Classifiers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") != null && !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                Classifiers = await _context.Classifiers.Where(x => x.AccountClassifiers.Any(y => y.ClassifierId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            return RedirectToPage("Error");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Classifiers");
            }

            Models.Classifier classifier = await _context.Classifiers.FindAsync(id);

            if (classifier != null && await _context.AccountClassifiers.FirstOrDefaultAsync(x => x.ClassifierId == classifier.Id && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                var path = Path.Combine(_environment.WebRootPath, "classifiers", $"{classifier.Model}.zip");

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                _context.Classifiers.Remove(classifier);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Classifiers");
        }
    }
}
