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
    public class MergeDatasetModel : PageModel
    {
        private readonly Models.Context _context;

        public MergeDatasetModel(Models.Context context)
        {
            _context = context;
        }
        
        [BindProperty]
        [Required]
        [DisplayName("Merged dataset name")]
        public string Name { get; set; }

        [BindProperty]
        [DisplayName("First dataset")]
        public int DatasetId1 { get; set; }

        [BindProperty]
        [DisplayName("Second dataset")]
        public int DatasetId2 { get; set; }

        public List<Models.Dataset> Datasets { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).Include(x => x.Articles).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.Datasets.FirstOrDefaultAsync(x => x.Name == Name && (x.AccountDatasets.FirstOrDefault(y => y.AccountId == HttpContext.Session.GetInt32("id")) != null)) != null)
            {
                ModelState.AddModelError("Name", "You already have a dataset with this name.");
            }

            if (DatasetId1 == DatasetId2)
            {
                ModelState.AddModelError("DatasetId2", "You must select two different datasets.");
            }

            if (!ModelState.IsValid)
            {
                Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).Include(x => x.Articles).ToListAsync();
                return Page();
            }

            if (await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == DatasetId1 && x.AccountId == HttpContext.Session.GetInt32("id")) == null || await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == DatasetId2 && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            List<Models.Article> queriedArticles = await _context.Articles.Where(x => x.DatasetId == DatasetId1 || x.DatasetId == DatasetId2).ToListAsync();
            List<Models.Article> combinedArticles = new List<Models.Article>();
            
            foreach (Models.Article article in queriedArticles)
            {
                combinedArticles.Add(new Models.Article() {
                    Abstract = article.Abstract,
                    Classification = article.Classification,
                    Pmid = article.Pmid,
                    Title = article.Title,
                    Date = article.Date
                });
            }

            Models.Dataset dataset = new Models.Dataset()
            {
                Name = Name,
                Articles = combinedArticles,
                Date = DateTime.Now
            };

            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == HttpContext.Session.GetInt32("id"));
            _context.Datasets.Add(dataset);
            _context.AccountDatasets.Add(new Models.AccountDataset() { Account = account, Dataset = dataset });
            await _context.SaveChangesAsync();
            return RedirectToPage("Datasets");
        }
    }
}
