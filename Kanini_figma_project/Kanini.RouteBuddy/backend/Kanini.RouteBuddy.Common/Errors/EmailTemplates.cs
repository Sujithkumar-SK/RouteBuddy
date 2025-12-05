namespace Kanini.RouteBuddy.Common.Errors.MagicStrings;

public static class EmailTemplates
{
    public static string GetVerificationCodeEmail(string customerName, string otp)
    {
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>RouteBuddy Email Verification</h2>
            <p>Dear {customerName},</p>
            <p>Your verification code is: <strong>{otp}</strong></p>
            <p>This code will expire in 10 minutes.</p>
            <p>Best regards,<br/>RouteBuddy Team</p>
        </body>
        </html>";
    }

    public static string GetPasswordResetEmail(string customerName, string otp)
    {
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>RouteBuddy Password Reset</h2>
            <p>Dear {customerName},</p>
            <p>Your password reset code is: <strong>{otp}</strong></p>
            <p>This code will expire in 10 minutes.</p>
            <p>Best regards,<br/>RouteBuddy Team</p>
        </body>
        </html>";
    }
}