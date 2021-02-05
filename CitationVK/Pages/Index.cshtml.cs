using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CitationVK.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Models.Context _context;

        public IndexModel(Models.Context context)
        {
            _context = context;
        }

        public Models.Configuration Configuration { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Configuration = await _context.Configurations.FirstOrDefaultAsync(x => x.Id == 1);
            return Page();
        }
    }
}
