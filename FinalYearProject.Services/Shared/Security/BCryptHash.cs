using BC = BCrypt.Net.BCrypt;

namespace FinalYearProject.Services.Shared.Security;


/// <summary>
/// Returns the bycrypt class and adds addional two methods hash and verify  from the bycrypt.
/// </summary>
public static class BCryptHash
{
    public static string Hash(string text)
    {
        if(text == null) throw new ArgumentNullException("text");

        return BC.HashPassword(text);
    }

    public static bool Verify(string hash, string text)
    {
        return BC.Verify(text, hash);
    }
}
