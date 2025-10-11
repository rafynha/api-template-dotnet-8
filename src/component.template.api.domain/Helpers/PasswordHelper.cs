using System.Security.Cryptography;
using System.Text;

namespace component.template.api.domain.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("A senha não pode ser nula ou vazia.", nameof(password));

        using (var sha256 = SHA256.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string hashedPassword, string inputPassword) => 
        hashedPassword == HashPassword(inputPassword);    

    public static bool CompareWithLastPassword(string oldPassword, string newPassword) =>    
        oldPassword.Equals(HashPassword(newPassword));    
}
