using System.Text.RegularExpressions;
using Kanini.RouteBuddy.Application.Dto.Vendor;

namespace Kanini.RouteBuddy.Application.Validators;

public static class VendorValidator
{
    public static List<string> ValidateRegistration(VendorRegistrationDto dto)
    {
        var errors = new List<string>();

        // Email validation
        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(dto.Email))
            errors.Add("Invalid email format");
        else if (dto.Email.Length > 150)
            errors.Add("Email cannot exceed 150 characters");

        // Password validation
        if (string.IsNullOrWhiteSpace(dto.Password))
            errors.Add("Password is required");
        else if (!IsValidPassword(dto.Password))
            errors.Add("Password must be at least 8 characters with uppercase, lowercase, number, and special character");

        // Phone validation
        if (string.IsNullOrWhiteSpace(dto.Phone))
            errors.Add("Phone number is required");
        else if (!IsValidIndianPhone(dto.Phone))
            errors.Add("Invalid Indian mobile number format");

        // Agency name validation
        if (string.IsNullOrWhiteSpace(dto.AgencyName))
            errors.Add("Agency name is required");
        else if (dto.AgencyName.Length < 2 || dto.AgencyName.Length > 150)
            errors.Add("Agency name must be between 2 and 150 characters");

        // Owner name validation
        if (string.IsNullOrWhiteSpace(dto.OwnerName))
            errors.Add("Owner name is required");
        else if (!IsValidName(dto.OwnerName))
            errors.Add("Owner name contains invalid characters");

        // Business license validation
        if (string.IsNullOrWhiteSpace(dto.BusinessLicenseNumber))
            errors.Add("Business license number is required");
        else if (!IsValidLicenseNumber(dto.BusinessLicenseNumber))
            errors.Add("Invalid business license number format");

        // Fleet size validation
        if (dto.FleetSize < 1 || dto.FleetSize > 1000)
            errors.Add("Fleet size must be between 1 and 1000");

        return errors;
    }

    public static List<string> ValidateProfileUpdate(UpdateVendorProfileDto dto)
    {
        var errors = new List<string>();

        if (!IsValidEmail(dto.Email))
            errors.Add("Invalid email format");

        if (!IsValidIndianPhone(dto.Phone))
            errors.Add("Invalid phone number format");

        if (string.IsNullOrWhiteSpace(dto.AgencyName) || dto.AgencyName.Length > 100)
            errors.Add("Agency name is required and cannot exceed 100 characters");

        if (dto.FleetSize < 1 || dto.FleetSize > 1000)
            errors.Add("Fleet size must be between 1 and 1000");

        return errors;
    }

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private static bool IsValidPassword(string password) =>
        password.Length >= 8 && 
        Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

    private static bool IsValidIndianPhone(string phone) =>
        Regex.IsMatch(phone, @"^(\+91[\-\s]?)?[6-9]\d{9}$");

    private static bool IsValidName(string name) =>
        Regex.IsMatch(name, @"^[a-zA-Z\s\.]+$");

    private static bool IsValidLicenseNumber(string license) =>
        Regex.IsMatch(license, @"^[A-Z0-9\-\/]+$");
}