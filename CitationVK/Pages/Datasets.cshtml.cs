using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class DatasetsModel : PageModel
    {
        private readonly Models.Context _context;

        public DatasetsModel(Models.Context context)
        {
            _context = context;
        }

        public IList<Models.Dataset> Datasets { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") != null && !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).Include(x => x.Articles).ToListAsync();
                return Page();
            }

            return RedirectToPage("Error");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Datasets");
            }

            Models.Dataset dataset = await _context.Datasets.FindAsync(id);

            if (dataset != null && await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == dataset.Id && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                _context.Datasets.Remove(dataset);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Datasets");
        }

        public async Task<IActionResult> OnPostDownloadAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Datasets");
            }

            Models.Dataset dataset = await _context.Datasets.FindAsync(id);

            if (dataset != null && await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == dataset.Id && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                List<Models.Article> articles = await _context.Articles.Where(x => x.DatasetId == dataset.Id).ToListAsync();
                StringBuilder fileContents = new StringBuilder();

                foreach (Models.Article article in dataset.Articles)
                {
                    fileContents.Append($"{article.Pmid},{(article.Classification ? "1" : "0")}\r\n");
                }
                
                return File(Encoding.UTF8.GetBytes(fileContents.ToString()), "text/csv", $"{dataset.Name}.csv");
            }

            return RedirectToPage("Error");
        }
    }
}
