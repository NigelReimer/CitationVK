using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class AccountsModel : PageModel
    {
        private readonly Models.Context _context;

        public AccountsModel(Models.Context context)
        {
            _context = context;
        }

        public List<Models.Account> Accounts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.Get("isAdmin") == null || !BitConverter.ToBoolean(HttpContext.Session.Get("isAdmin")))
            {
                return RedirectToPage("Error");
            }

            Accounts = await _context.Accounts.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Accounts");
            }

            Models.Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (account != null && account.Id != HttpContext.Session.GetInt32("id"))
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Accounts");
        }
    }
}
