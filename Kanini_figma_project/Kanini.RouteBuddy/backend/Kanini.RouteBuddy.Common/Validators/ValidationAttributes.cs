using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kanini.RouteBuddy.Common.Validators;

public class IndianPhoneAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string phone)
            return Regex.IsMatch(phone, @"^(\+91[\-\s]?)?[6-9]\d{9}$");
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid Indian mobile number (10 digits starting with 6-9)";
    }
}

public class BusinessLicenseAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string license)
            return Regex.IsMatch(license, @"^[A-Z0-9\-\/]{5,50}$");
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be 5-50 characters containing only uppercase letters, numbers, hyphens, and forward slashes";
    }
}

public class VendorNameAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string name)
            return Regex.IsMatch(name, @"^[a-zA-Z0-9\s\-\.&]{2,150}$");
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be 2-150 characters containing only letters, numbers, spaces, hyphens, dots, and ampersands";
    }
}

public class PersonNameAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string name)
            return Regex.IsMatch(name, @"^[a-zA-Z\s\.]{2,100}$");
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be 2-100 characters containing only letters, spaces, and dots";
    }
}

public class StrongPasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string password)
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&)";
    }
}