using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Identity;

//made by Alex, utilising adapted code from Basket.cshtml.cs
namespace currentworkingsassyplanner.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly SPContext _spContext;
        private readonly UserManager<IdentityUser> _userManager;
        public IList<OrderConf> OrderInfo { get; private set; }
        
        public OrderConfirmationModel(SPContext spContext, UserManager<IdentityUser> userManager)
        {
            _spContext = spContext;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            //var cookie = HttpContext.Request.Cookies["SassyPlannerCookie"];

            if (user != null)
            {
                var customer = _spContext.Customers.FromSqlRaw("SELECT * FROM Customer WHERE Email = {0}", user.Email).ToList().FirstOrDefault();
                if (customer != null)
                {
                    OrderInfo = _spContext.OrderConfs.FromSqlRaw(
                    "SELECT Customer.BasketID, CustomerOrder.OrderID, CustomerOrder.OrderPrice AS OrderTotal, " +
                    "SYSDATETIME() AS OrderDate, Customer.AddressLineOne + Customer.AddressLineTwo + " +
                    "Customer.City AS DeliveryAddress FROM Customer INNER JOIN CustomerOrder ON " +
                    "Customer.CustomerID = CustomerOrder.CustomerID " +
                    "WHERE BasketID = {0}", customer.BasketID).ToList();
                }
            } else
            {
                Console.WriteLine("BasketID does not exist");
            }
        }
    }
}
