using System.Security.Cryptography;
using System.Text;

namespace FinalYearProject.Data.Utilities;


public class Password
{

    public static string Generate { get { return GeneratePassword(); } }

    private static string GeneratePassword(int length = 20)
    {
        if (length < 8)
        {
            throw new ArgumentException("Password length must be at least 8 characters.");
        }

        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string specialChars = "!@#$%&*_+";

        var randomChars = new StringBuilder();
        randomChars.Append(upperCaseChars[GetRandomNumber(upperCaseChars.Length)]);
        randomChars.Append(lowerCaseChars[GetRandomNumber(lowerCaseChars.Length)]);
        randomChars.Append(digitChars[GetRandomNumber(digitChars.Length)]);
        randomChars.Append(specialChars[GetRandomNumber(specialChars.Length)]);

        string? allChars = upperCaseChars + lowerCaseChars + digitChars + specialChars;
        for (int i = randomChars.Length; i < length; i++)
        {
            randomChars.Append(allChars[GetRandomNumber(allChars.Length)]);
        }

        string password = new string(randomChars.ToString().ToCharArray().OrderBy(s => GetRandomNumber(100)).ToArray());
        if (password.Length <= 8)
        {
            password += GetRandomNumber(100);
        }
        return password;
    }

    private static int GetRandomNumber(int max)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomNumber = new byte[1];
            rng.GetBytes(randomNumber);
            return randomNumber[0] % max;
        }
    }
}
