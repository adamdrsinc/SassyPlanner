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
    //Made by Adam Sinclair 
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        
        private readonly currentworkingsassyplanner.Data.SPContext _context;

        public IndexModel(currentworkingsassyplanner.Data.SPContext context)
        {
            _context = context;
        }

        public IList<Product> ProductTBL { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                ProductTBL = await _context.Products.ToListAsync();
            }
        }
    }
}
