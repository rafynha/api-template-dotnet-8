namespace component.template.api.domain.Helpers;

public static class EmailHelper
{
    public static bool IsValidEmail(string email)
    {
        try
        {
            // Usa a classe System.Net.Mail para validar o email
            var mailAddress = new System.Net.Mail.MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}