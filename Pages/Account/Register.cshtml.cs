// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

//Overhauled by Adam Sinclair, scaffolded from Razor Pages.

namespace currentworkingsassyplanner.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private SPContext _context;
        //public CheckoutCustomer checkoutCustomer = new CheckoutCustomer();
        public Basket basket = new Basket();
        private int customersNewID = 1;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            SPContext theContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = theContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(50), MinLength(1), MaxLength(50)]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name may only contain letters.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50), MinLength(1), MaxLength(50)]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Surname may only contain letters.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress, MaxLength(100)]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(255), MinLength(1), MaxLength(255)]
            [Display(Name = "First Line Address")]
            public string FirstLineAddress { get; set; }

            [Required]
            [StringLength(255), MinLength(1), MaxLength(255)]
            [Display(Name = "Second Line Address")]
            public string SecondLineAddress { get; set; }

            [Required]
            [StringLength(50), MinLength(1), MaxLength(50)]
            [Display(Name = "City")]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "City may only contain letters.")]
            public string City { get; set; }

            [Required]
            [StringLength(10), MinLength(4)]
            [RegularExpression("^([A-Z][A-HJ-Y]?\\d[A-Z\\d]? ?\\d[A-Z]{2}|GIR ?0A{2})$", ErrorMessage = "Invalid Postcode entered.")]
            [Display(Name = "Postcode")]
            public string Postcode { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        NewBasket();
                        AddNewUser(
                            Input.FirstName,
                            Input.LastName,
                            Input.Email,
                            Input.FirstLineAddress,
                            Input.SecondLineAddress,
                            Input.City,
                            Input.Postcode,
                            basket.BasketID
                            );
                        //NewCustomer(Input.Email);
                        await _userManager.AddToRoleAsync(user, "Member");
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        //Creates a new basket.
        public void NewBasket()
        {
            var allBaskets = _context.Baskets.FromSqlRaw("SELECT * FROM Basket").ToList();
            var lastBasketMade = new Basket();
            if (allBaskets == null || allBaskets.Count == 0) { basket.BasketID = 1; }
            else
            {
/*                lastBasketMade = _context.Baskets.FromSqlRaw("SELECT * FROM Basket")
                .OrderByDescending(b => b.BasketID)
                .FirstOrDefault();

                allBaskets.First().BasketID*/

                basket.BasketID = allBaskets.Last().BasketID + 1;
                
            }

            basket.CookieID = "not-used";

            

/*            //If there are no baskets, create one. The user that is being created will use this new basket.
            if (currentBasket == null)
            {
                basket.BasketID = 1;
            }
            //If there are already baskets, get the highest basket ID, increment by one. This will be given
            //to the new user.
            else
            {
                basket.BasketID = currentBasket.BasketID + 1;
            }
*/
            _context.Baskets.Add(basket);
            _context.SaveChanges();
        }

        private void AddNewUser(string firstName, string lastName, string email, string firstLineAddress,
            string secondLineAddress, string city, string postcode, int basketID)
        {
            var lastUserInUsersTable = _context.Customers.FromSqlRaw("SELECT * FROM Customer")
                .OrderByDescending(customer => customer.CustomerID)
                .FirstOrDefault();

            var newUserID = 1;
            if (lastUserInUsersTable != null) newUserID = lastUserInUsersTable.CustomerID + 1;
            customersNewID = newUserID;


            Customer newCustomer = new Customer
            {
                CustomerID = newUserID,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                AddressLineOne = firstLineAddress,
                AddressLineTwo = secondLineAddress,
                City = city,
                Postcode = postcode,
                BasketID = basketID
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
        }

        //Links the new customer and their new basket, and then adds the customer to the CheckoutCustomers table.
/*        public void NewCustomer(string Email)
        {

            checkoutCustomer.CustomerID = customersNewID;
            checkoutCustomer.BasketID = basket.BasketID;

            _context.CheckoutCustomers.Add(checkoutCustomer);
            _context.SaveChanges();
        }*/
    }
}
