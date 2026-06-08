using Bcrypt = BCrypt.Net.BCrypt;

namespace FinalYearProject.Data.Extensions;

public static class BCryptExtension
{
    /// <summary>
    /// Hash string with BCrypt.Net-Next package
    /// <returns>Hashed string of the original string.</return>
    /// </summary>
    public static string Hash(this string value)
    {
        string saltKey = Bcrypt.GenerateSalt(12);
        return Bcrypt.HashPassword(value, saltKey);
    }

    /// <summary>
    /// Verify if hashed string match with the provided string. The extended string is the string to verify while the argument is the hashed string.
    /// </summary>
    /// <param name="password">string to verify</param>
    /// <param name="hashedPassword">Hashed string</param>
    /// <returns></returns>
    public static bool Verify(this string password, string hashedPassword)
    {
        return Bcrypt.Verify(password, hashedPassword);
    }
}
