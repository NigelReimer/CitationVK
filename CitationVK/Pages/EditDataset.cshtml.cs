using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class EditDatasetModel : PageModel
    {
        private readonly Models.Context _context;

        public EditDatasetModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        public string Name { get; set; }

        public Models.Dataset Dataset { get; set; }

        public List<Models.Article> Articles { get; set; }

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

            Dataset = await _context.Datasets.FirstOrDefaultAsync(x => x.Id == id);

            if (Dataset == null || await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == Dataset.Id && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            Name = Dataset.Name;
            Articles = await _context.Articles.Where(x => x.DatasetId == Dataset.Id).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRenameAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Datasets");
            }

            Models.Dataset dataset = await _context.Datasets.FirstOrDefaultAsync(x => x.Id == id);

            if (await _context.Datasets.FirstOrDefaultAsync(x => x.Name == Name && (x.AccountDatasets.FirstOrDefault(y => y.AccountId == HttpContext.Session.GetInt32("id")) != null)) != null)
            {
                ModelState.AddModelError("Name", "You already have a dataset with this name.");
                Dataset = await _context.Datasets.FirstOrDefaultAsync(x => x.Id == id);
                Name = Dataset.Name;
                Articles = await _context.Articles.Where(x => x.DatasetId == Dataset.Id).ToListAsync();
                return Page();
            }

            if (dataset != null && await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == dataset.Id && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                _context.Attach(dataset).State = EntityState.Modified;
                dataset.Name = Name;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Datasets");
        }

        public async Task<IActionResult> OnPostReverseAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Error");
            }

            Models.Article article = await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);

            if (article != null && await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == article.DatasetId && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                _context.Attach(article).State = EntityState.Modified;
                article.Classification = !article.Classification;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("EditDataset", new { id = article.DatasetId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Error");
            }

            Models.Article article = await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);

            if (article != null && await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == article.DatasetId && x.AccountId == HttpContext.Session.GetInt32("id")) != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("EditDataset", new { id = article.DatasetId });
        }
    }
}
