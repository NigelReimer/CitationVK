using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class EditIndexModel : PageModel
    {
        private readonly Models.Context _context;

        public EditIndexModel(Models.Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Configuration Configuration { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Configuration = await _context.Configurations.FirstOrDefaultAsync(x => x.Id == 1);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Configuration).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
