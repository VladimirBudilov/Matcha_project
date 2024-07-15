using System.Security.Cryptography;

namespace BLL.Helpers;

public class PasswordManager
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
    public bool VerifyHashedPassword(string newHashedPassword, string oldHashedPassword)
    {
        return newHashedPassword == oldHashedPassword;
    }


    public static string GeneratePassword()
    {
        const string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{};':,.<>?";
        var random = new RNGCryptoServiceProvider();
        var passwordChars = new List<char>();

        passwordChars.Add(GetRandomChar(uppercaseLetters, random));
        passwordChars.Add(GetRandomChar(specialChars, random));
        passwordChars.AddRange(Enumerable.Range(0, 3).Select(x => GetRandomChar(numbers, random)));
        passwordChars.AddRange(Enumerable.Range(0, 3).Select(x => GetRandomChar(lowercaseLetters, random)));
        passwordChars = passwordChars.OrderBy(x => Guid.NewGuid()).ToList();

        return new string(passwordChars.ToArray());
    }

    private static char GetRandomChar(string validChars, RNGCryptoServiceProvider rng)
    {
        byte[] uintBuffer = new byte[sizeof(uint)];
        rng.GetBytes(uintBuffer);
        uint num = BitConverter.ToUInt32(uintBuffer, 0);
        return validChars[(int)(num % (uint)validChars.Length)];
    }
}