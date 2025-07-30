using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ToDoList.Application;

public class Sha256PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}