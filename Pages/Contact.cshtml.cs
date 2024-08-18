using currentworkingsassyplanner.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace currentworkingsassyplanner.Pages
{
    //Made by Adam Sinclair
    public class ContactModel : PageModel
    {

        [BindProperty, Required(ErrorMessage = "Name is required."), RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name may only contain letters.")]
        public string m_Name { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Subject is required.")]
        public string m_Subject { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Valid Email is required."), EmailAddress]
        public string m_Email { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Message is required.")]
        public string m_Message { get; set; } = string.Empty;

        public string m_SuccessMessage { get; set; } = string.Empty;
        public string m_ErrorMessage { get; set; } = string.Empty;


        public void OnPostSendEmailAsync()
        {
            if(!ModelState.IsValid)
            {
                m_ErrorMessage = "The email was not successfully sent.";
            }
            else
            {
                m_SuccessMessage = "The email was successfully sent.";

                EmailSender sender = new EmailSender();
                sender.SendEmail(m_Name, m_Subject, m_Email, m_Message);
            }


        }

        public void OnGet()
        {
        }
    }
}
