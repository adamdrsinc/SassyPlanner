using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Authorization;

namespace currentworkingsassyplanner.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly currentworkingsassyplanner.Data.SPContext _context;

        public DeleteModel(currentworkingsassyplanner.Data.SPContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Product ProductTBL { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var producttbl = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == id);

            if (producttbl == null)
            {
                return NotFound();
            }
            else 
            {
                ProductTBL = producttbl;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var producttbl = await _context.Products.FindAsync(id);

            if (producttbl != null)
            {
                ProductTBL = producttbl;
                _context.Products.Remove(ProductTBL);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
