using System.Net.Mail;
using System.Net;
using System.Xml.Linq;

namespace currentworkingsassyplanner.Email
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(string p_Name, string p_Subject, string p_Email, string p_Message)
        {
            var spEmail = "sassyplannerenquiries@outlook.com";
            var pw = "ThePlanner1!";

            SmtpClient client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(spEmail, pw)
            };

            client.SendMailAsync(
                new MailMessage(
                    from: spEmail,
                    to: spEmail,
                    subject: p_Subject,
                    "Name: " + p_Name + "\n" +
                    "Email: " + p_Email + "\n" +
                    "Message: " + p_Message
                    )
                );
        }
    }
}
