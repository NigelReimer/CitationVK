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
    public class ShareDatasetModel : PageModel
    {
        private readonly Models.Context _context;

        public ShareDatasetModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        [DisplayName("Dataset")]
        public int DatasetId { get; set; }

        [BindProperty]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public List<Models.Dataset> Datasets { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("id") == null || (HttpContext.Session.Get("isAdmin") != null && BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin"))))
            {
                return RedirectToPage("Error");
            }

            Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _context.AccountDatasets.FirstOrDefaultAsync(x => x.DatasetId == DatasetId && x.AccountId == HttpContext.Session.GetInt32("id")) == null)
            {
                return RedirectToPage("Error");
            }

            Models.Dataset dataset = await _context.Datasets.FirstOrDefaultAsync(x => x.Id == DatasetId);
            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == Email && !x.IsAdmin);

            if (account == null)
            {
                ModelState.AddModelError("Email", "No account with this email address exists.");
            }
            else if (await _context.Datasets.FirstOrDefaultAsync(x => x.Name == dataset.Name && (x.AccountDatasets.FirstOrDefault(y => y.AccountId == account.Id) != null)) != null)
            {
                ModelState.AddModelError("DatasetId", "User already has a dataset with this name.");
            }

            if (!ModelState.IsValid)
            {
                Datasets = await _context.Datasets.Where(x => x.AccountDatasets.Any(y => y.DatasetId == x.Id && y.AccountId == HttpContext.Session.GetInt32("id"))).ToListAsync();
                return Page();
            }

            _context.AccountDatasets.Add(new Models.AccountDataset() { Account = account, Dataset = dataset });
            await _context.SaveChangesAsync();
            return RedirectToPage("Datasets");
        }
    }
}
