using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace currentworkingsassyplanner.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly currentworkingsassyplanner.Data.SPContext _context;

        public CreateModel(currentworkingsassyplanner.Data.SPContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            m_AllProducts = GetAllProducts();
            return Page();
        }

        [BindProperty]
        public Product m_NewProduct { get; set; }

        private List<Product> m_AllProducts = new List<Product>();
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            m_AllProducts = GetAllProducts();

            if (!ModelState.IsValid || _context.Products == null || m_NewProduct == null)
            {
                return Page();
            }

            int productID;
            if(m_AllProducts.Count == 0)
            {
                productID = 1;
            }
            else
            {
                int productCount = m_AllProducts.Count;
                Product lastProduct = m_AllProducts[productCount - 1];
                int lastProductID = lastProduct.ProductID;

                productID = lastProductID + 1;
            }

            m_NewProduct.ProductID = productID;

            foreach(var file in Request.Form.Files)
            {
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                m_NewProduct.ProductImageData = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }

            _context.Products.Add(m_NewProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private List<Product> GetAllProducts()
        {
            return _context.Products.FromSqlRaw("SELECT * FROM Product").ToList();

        }
    }
}
