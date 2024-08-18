using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

//initially made by Alex, mostly rewritten by Sam
namespace currentworkingsassyplanner.Pages
{
    public class BasketModel : PageModel
    {
        private readonly SPContext _spContext;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public bool isLoggedIn { get; set; }

        [BindProperty]
        public List<string> prodName { get; set; }
        [BindProperty]
        public List<decimal> prodPrice { get; set; }

        public IList<BasketItem> Items { get; private set; }
        public decimal Total;
        public long AmountPayable;

        public BasketModel(SPContext spContext, UserManager<IdentityUser> userManager)
        {
            _spContext = spContext;
            _userManager = userManager;
            prodName = new List<string>();
            prodPrice = new List<decimal>();
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user !=null)
            {
                isLoggedIn = true;
                var customer = _spContext.Customers.FromSqlRaw("SELECT * FROM Customer WHERE Email = {0}", user.Email).ToList().FirstOrDefault();

                if (customer != null)
                {

                    Items = await _spContext.BasketItems
                                        .Where(b => b.BasketID == customer.BasketID)
                                        .ToListAsync();


                    if (Items != null && Items.Count > 0)
                    {
                        foreach (var item in Items)
                        {

                            var prod = _spContext.Products.FromSqlRaw("SELECT * FROM Product WHERE ProductID = {0}", item.ProductID).FirstOrDefault();
                            if (prod != null)
                            {
                                prodName.Add(prod.ProductName);
                                Total += (item.Quantity * prod.ProductPrice);
                                prodPrice.Add(item.Quantity * prod.ProductPrice);
                            }
                        }
                    }
                }
            }
            else
            {
                isLoggedIn = false;
            }

            


        }

        public async Task OnPostDeleteAsync(int basketItemID)
        {

            var allItems = _spContext.BasketItems.FromSqlRaw("SELECT * FROM BasketItem WHERE BasketItemID = {0}", basketItemID);
            BasketItem item = new BasketItem();
            if(allItems !=  null)
            {
                item = allItems.FirstOrDefault();
                _spContext.BasketItems.Remove(item);

            }

            //Save the changes, then refresh the basket table.
            await _spContext.SaveChangesAsync();
            //Items = GetCheckoutItems(customer);
            await OnGetAsync();
        }

        //public async Task<IActionResult> OnPostBuyAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user != null)
        //    {
        //        var customer = _spContext.Customers.FromSqlRaw("SELECT * FROM Customer WHERE Email = {0}", user.Email).ToList().FirstOrDefault();

        //        if (customer == null)
        //        {
        //            // Handle case where customer is not found
        //            return Page();
        //        }

        //        var order = new CustomerOrder
        //        {
        //            CustomerID = customer.CustomerID,
        //            OrderPrice = Total
        //        };

        //        _spContext.Orders.Add(order);
        //        await _spContext.SaveChangesAsync();

        //        var basketItems = await _spContext.Ba sketItems
        //                                          .Where(b => b.BasketID == customer.BasketID)
        //                                          .ToListAsync();

        //        foreach (var item in basketItems)
        //        {
        //            _spContext.BasketItems.Remove(item);
        //        }

        //        await _spContext.SaveChangesAsync();
        //    }
        //    return RedirectToPage("/OrderConfirmation");
        //}
    }
}
