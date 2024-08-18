using currentworkingsassyplanner.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Nodes;
using System.Text;
using currentworkingsassyplanner.Models;
using System;
using Microsoft.EntityFrameworkCore;

namespace currentworkingsassyplanner.Pages
{
    [IgnoreAntiforgeryToken]
    public class CheckOutModel : PageModel
    {
        private readonly SPContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public string PaypalClientID { get; set; } = "";
        private string PaypalSecret { get; set; } = "";
        public string PaypalUrl { get; set; } = "";

        public decimal m_TotalPrice { get; set; } = 0;
        public IList<BasketItem> Items { get; private set; } = new List<BasketItem>();

        //public Product testProduct { get; set; }

        public CheckOutModel(SPContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;

            //Obtaining paypal information from appsettings.json
            PaypalClientID = configuration["PaypalSettings:ClientID"];
            PaypalSecret = configuration["PaypalSettings:Secret"];
            PaypalUrl = configuration["PaypalSettings:URL"];

        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var customer = _context.Customers.FromSqlRaw("SELECT * FROM Customer WHERE Email = {0}", user.Email).ToList().FirstOrDefault();
                if (customer != null)
                {
                    Items = await _context.BasketItems
                                        .Where(b => b.BasketID == customer.BasketID)
                                        .ToListAsync();


                    if (Items != null && Items.Count > 0)
                    {
                        foreach (var item in Items)
                        {

                            m_TotalPrice += item.ItemPrice;
                        }
                    }
                }

            }


            /*testProduct = new Product
            {
                ProductName = "blah",
                ProductID = 10,
                ProductImageData = new byte[] {},
                ProductImageDescription = "dd",
                ProductPrice = 13.99m,
                ProductType = "something"
            };*/
        }

        //Code adapted from BoostMyTool, 2023.

        /// <summary>
        /// What happens when the user presses the PayPal button on the Checkout page.
        /// </summary>
        /// <param name="data">JSON Object containing data regarding the total cost of the items in user's basket.</param>
        /// <returns></returns>
        public JsonResult OnPostCreateOrder([FromBody] JsonObject data)
        {

            var total = data["total"];
            if (total == null) return new JsonResult("");

            //Creating JSON Object in format PayPal requires for handling purchasing.

            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            //Setting the currency and the amount.
            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "GBP");
            amount.Add("value", total.ToString());

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            //Token for connecting to paypal.
            string accessToken = GetPaypalAccessToken();

            //Url for connecting to paypal.
            string url = PaypalUrl + "/v2/checkout/orders";

            //Order ID for paypal.
            string orderID = "";

            //Connecting to paypal and passing in cost, currency, and required items such as the accessToken.
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        orderID = jsonResponse["id"]?.ToString() ?? "";
                    }
                }

            }


            var response = new
            {
                id = orderID
            };


            return new JsonResult(response);

        }

        /// <summary>
        /// What happens when the user approves their purchase in the pop-up window that PayPal provides.
        /// </summary>
        /// <param name="data">JSON Object containing data regarding the order.</param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCompleteOrder([FromBody] JsonObject data)
        {
            //If the data passed in from the body is null, return an empty JSONResult.
            if (data == null || data["orderID"] == null) return new JsonResult("");

            var orderID = data["orderID"]!.ToString();

            var accessToken = GetPaypalAccessToken();

            string url = PaypalUrl + "/v2/checkout/orders/" + orderID + "/capture";

            //Connecting to paypal and completing order.
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            //TODO: ADD TO ORDER HISTORY
                            //TODO: REMOVE ITEMS FROM BASKET

                            return new JsonResult("success");
                        }
                    }
                }
            }

            //Return if order completion was not successful.
            return new JsonResult("");
        }

        /// <summary>
        /// What hapepns when the user cancels their order.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult OnPostCancelOrder([FromBody] JsonObject data)
        {
            if (data == null || data["orderID"] == null) return new JsonResult("");

            var orderID = data["orderID"]!.ToString();

            return new JsonResult("");
        }

        /// <summary>
        /// Obtaining the access token for PayPal authorisation.
        /// </summary>
        /// <returns></returns>
        private string GetPaypalAccessToken()
        {
            string accessToken = "";

            string url = PaypalUrl + "/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                string credentials64 =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientID + ":" + PaypalSecret));

                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

                var responseTask = client.SendAsync(requestMessage);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var strResponse = readTask.Result;

                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
            }

            return accessToken;
        }

        //End of adapted code.

    }
}
