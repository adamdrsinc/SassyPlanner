using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using currentworkingsassyplanner.Models;
using Microsoft.EntityFrameworkCore;

namespace currentworkingsassyplanner.Pages
{
    //Sams code
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly currentworkingsassyplanner.Data.SPContext _context;

        [BindProperty]
        public string? m_Email { get; set; }

        public bool? m_IsValid = null;

        public string? m_Discount = null;

        [BindProperty]
        public IList<Product> m_ProductList { get; set; } = default!;

        public void getProdimg()
        {
            m_ProductList = _context.Products.FromSqlRaw("SELECT * FROM Product").ToList();

            // Shuffle the list
            Random rng = new Random();
            int n = m_ProductList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Product value = m_ProductList[k];
                m_ProductList[k] = m_ProductList[n];
                m_ProductList[n] = value;
            }

            // Take the first 6 unique elements
            m_ProductList = m_ProductList.Distinct().Take(6).ToList();
        }

        //The discount code does not actually work, this is a desired feature
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            //verify email syntax
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            if (m_Email != null)
            {
                m_IsValid = regex.IsMatch(m_Email);
                if (m_IsValid == true)
                {
                    //set discount code
                    m_Discount = "SASSY2";
                }
            }
            getProdimg();
            return Page();
        }

        public IndexModel(ILogger<IndexModel> logger, currentworkingsassyplanner.Data.SPContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            getProdimg();
        }
    }
}
