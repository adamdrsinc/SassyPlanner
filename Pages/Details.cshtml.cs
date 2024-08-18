using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//Initially written by Ajay, Heavily changed by sam (with contribution from Adam)

namespace currentworkingsassyplanner.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly SPContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public decimal price { get; set; }

        public DetailsModel(SPContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Product ProductTBL { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ProductTBL = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == id);

            if (ProductTBL == null)
            {
                return NotFound();
            }
            price = ProductTBL.ProductPrice;
            

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity, string personalisationText, string spiralColour, string startMonth, string internalPages, string plannerSize, string m_Message)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user!=null)
            {
                ProductTBL = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == productId);
                if (ProductTBL != null) price = ProductTBL.ProductPrice;

                var customer = _context.Customers.FromSqlRaw("SELECT * FROM Customer WHERE Email = {0}", user.Email).ToList().FirstOrDefault();

                //List<BasketItem> ids = _context.BasketItems.FromSqlRaw("SELECT BasketItemID FROM BasketItem").ToList();
                //foreach (var id in ids) 
                //{
                //    continue;
                //}

                if (customer != null)
                {

                    // TODO: swap the line underneath with this code to allow adding the same product with different features
                    // (doesnt currently work due to ProductID not allowing duplicates.
                    var item = _context.BasketItems.FirstOrDefault(x =>
                    x.ProductID == productId &&
                    x.BasketID == customer.BasketID &&
                    x.Personalisation == personalisationText &&
                    x.SpiralColour == spiralColour &&
                    x.StartMonth == startMonth &&
                    x.InternalPages == internalPages &&
                    x.PlannerSize == plannerSize &&
                    x.AdditionalInfo == m_Message);

                    //var item = _context.BasketItems.FromSqlRaw("SELECT * FROM BasketItem WHERE ProductID = {0} AND " +
                    //    "BasketID = {1}", productId, customer.BasketID).ToList().FirstOrDefault();

                    if (item == null)
                    {
                        if (m_Message == null)
                        {
                            m_Message = "";
                        }
                        BasketItem basketItem = new BasketItem
                        {

                            BasketID = customer.BasketID,
                            ProductID = productId,
                            Quantity = quantity,
                            ItemPrice = price,
                            Personalisation = personalisationText,
                            SpiralColour = spiralColour,
                            StartMonth = startMonth,
                            InternalPages = internalPages,
                            PlannerSize = plannerSize,
                            AdditionalInfo = m_Message
                        };

                        _context.BasketItems.Add(basketItem);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //item.Quantity += 1;
                        //_context.Attach(item).State = EntityState.Modified;
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            throw new Exception($"Basket not found!", ex);
                        }
                    }

                }
            }

            
            return RedirectToAction("OnGetAsync", new { id = productId });


        }
    }
}

