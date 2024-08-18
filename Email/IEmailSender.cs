namespace currentworkingsassyplanner.Email
{
    public interface IEmailSender
    {
        public void SendEmail(string p_Name, string p_Subject, string p_Email, string p_Message);
    }
}
