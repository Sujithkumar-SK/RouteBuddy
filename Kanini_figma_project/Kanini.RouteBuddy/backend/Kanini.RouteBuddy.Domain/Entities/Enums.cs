namespace Kanini.RouteBuddy.Domain.Enums;

public enum UserRole
{
    Customer = 1,
    Vendor = 2,
    Admin = 3,
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
}

public enum VendorStatus
{
    PendingApproval = 0,
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Rejected = 4,
}

public enum BusStatus
{
    PendingApproval = 0,
    Active = 1,
    Inactive = 2,
    Maintenance = 3,
    Rejected = 4,
}

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
}

public enum ScheduleStatus
{
    Departed = 0,
    Scheduled = 1,
    Cancelled = 2,
    Completed = 3,
    Delayed = 4,
}

public enum PaymentMethod
{
    Mock = 1,
    UPI = 2,
    Card = 3,
    NetBanking = 4,
}

public enum PaymentStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3,
    Refunded = 4,
}

public enum DocumentStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3,
}

public enum DocumentCategory
{
    BusinessLicense = 1,
    TaxRegistration = 2,
    InsuranceCertificate = 3,
    OwnerIdentity = 4,
    BankDetails = 5,
}

public enum BusType
{
    AC = 1,
    NonAC = 2,
    Sleeper = 3,
    SemiSleeper = 4,
    Volvo = 5,
    Luxury = 6,
}

public enum SeatType
{
    Seater = 1,
    SleeperLower = 2,
    SleeperUpper = 3,
    SemiSleeper = 4,
    SingleSeater = 5,
    SingleSemiSeater = 6,
}

public enum SeatPosition
{
    Window = 1,
    Aisle = 2,
    Middle = 3,
}

public enum PriceTier
{
    Base = 1,
    Premium = 2,
    Luxury = 3,
}

[Flags]
public enum BusAmenities
{
    None = 0,
    AC = 1,
    WiFi = 2,
    Charging = 4,
    Blanket = 8,
    Pillow = 16,
    Meals = 32,
    Washroom = 64,
    USB = 128,
    Reading_Light = 256,
    Entertainment = 512,
    Reclining_Seats = 1024,
    Emergency_Exit = 2048,
}
