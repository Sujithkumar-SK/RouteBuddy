using System.Text.RegularExpressions;

namespace Kanini.RouteBuddy.Common.Validators;

public static class PasswordValidator
{
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        var hasUpper = Regex.IsMatch(password, @"[A-Z]");
        var hasLower = Regex.IsMatch(password, @"[a-z]");
        var hasNumber = Regex.IsMatch(password, @"[0-9]");
        var hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]");

        return hasUpper && hasLower && hasNumber && hasSpecial;
    }

    public static string GetPasswordRequirements()
    {
        return "Password must contain at least 8 characters with 1 uppercase, 1 lowercase, 1 number, and 1 special character";
    }
}