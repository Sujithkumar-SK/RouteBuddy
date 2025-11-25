using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.DatabaseContext;

public class RouteBuddyDatabaseContext(DbContextOptions<RouteBuddyDatabaseContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<VendorDocument> VendorDocuments { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<RouteStop> RouteStops { get; set; }
    public DbSet<SeatLayoutTemplate> SeatLayoutTemplates { get; set; }
    public DbSet<SeatLayoutDetail> SeatLayoutDetails { get; set; }
    public DbSet<BusSchedule> BusSchedules { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BusPhoto> BusPhotos { get; set; }
    public DbSet<BookedSeat> BookedSeats { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BookingSegment> BookingSegments { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RouteBuddyDatabaseContext).Assembly);

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        modelBuilder
            .Entity<User>()
            .HasData(
                new User
                {
                    UserId = 1,
                    Email = "admin@routebuddy.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543210",
                    Role = UserRole.Admin,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new User
                {
                    UserId = 2,
                    Email = "customer1@gmail.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543211",
                    Role = UserRole.Customer,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new User
                {
                    UserId = 3,
                    Email = "vendor1@gmail.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543212",
                    Role = UserRole.Vendor,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Customers
        modelBuilder
            .Entity<Customer>()
            .HasData(
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = Gender.Male,
                    IsActive = true,
                    UserId = 2,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Vendors
        modelBuilder
            .Entity<Vendor>()
            .HasData(
                new Vendor
                {
                    VendorId = 1,
                    AgencyName = "Express Travels",
                    OwnerName = "Rajesh Kumar",
                    BusinessLicenseNumber = "BL123456",
                    OfficeAddress = "123 Main St, Chennai",
                    FleetSize = 25,
                    TaxRegistrationNumber = "TAX789012",
                    Status = VendorStatus.Active,
                    IsActive = true,
                    UserId = 3,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Routes
        modelBuilder
            .Entity<Route>()
            .HasData(
                new Route
                {
                    RouteId = 1,
                    Source = "Chennai",
                    Destination = "Bangalore",
                    Distance = 350.5m,
                    Duration = TimeSpan.FromHours(6),
                    BasePrice = 500m,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new Route
                {
                    RouteId = 2,
                    Source = "Mumbai",
                    Destination = "Pune",
                    Distance = 150.0m,
                    Duration = TimeSpan.FromHours(3),
                    BasePrice = 300m,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed SeatLayoutTemplates
        modelBuilder
            .Entity<SeatLayoutTemplate>()
            .HasData(
                new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = 1,
                    TemplateName = "AC Seater 2x2",
                    TotalSeats = 40,
                    BusType = BusType.AC,
                    Description = "Standard AC bus with 2x2 seating arrangement",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = 2,
                    TemplateName = "Sleeper 2x1",
                    TotalSeats = 36,
                    BusType = BusType.Sleeper,
                    Description = "Sleeper bus with upper and lower berths",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed SeatLayoutDetails for AC Seater 2x2 template
        var acSeaterSeats = new List<SeatLayoutDetail>();
        for (int row = 1; row <= 10; row++)
        {
            char rowLetter = (char)('A' + row - 1);
            for (int col = 1; col <= 4; col++)
            {
                var seatPosition = col switch
                {
                    1 or 4 => SeatPosition.Window,
                    2 or 3 => SeatPosition.Aisle,
                    _ => SeatPosition.Middle,
                };
                var priceTier =
                    seatPosition == SeatPosition.Window ? PriceTier.Premium : PriceTier.Base;

                acSeaterSeats.Add(
                    new SeatLayoutDetail
                    {
                        SeatLayoutDetailId = ((row - 1) * 4) + col,
                        SeatLayoutTemplateId = 1,
                        SeatNumber = $"{rowLetter}{col}",
                        SeatType = SeatType.Seater,
                        SeatPosition = seatPosition,
                        RowNumber = row,
                        ColumnNumber = col,
                        PriceTier = priceTier,
                        CreatedBy = "System",
                        CreatedOn = new DateTime(2024, 1, 1),
                    }
                );
            }
        }
        modelBuilder.Entity<SeatLayoutDetail>().HasData(acSeaterSeats);

        // Seed Buses
        modelBuilder
            .Entity<Bus>()
            .HasData(
                new Bus
                {
                    BusId = 1,
                    BusName = "Express Deluxe",
                    BusType = BusType.AC,
                    TotalSeats = 40,
                    RegistrationNo = "TN01AB1234",
                    RegistrationPath = "/docs/reg1.pdf",
                    Status = BusStatus.Active,
                    Amenities = BusAmenities.AC | BusAmenities.WiFi,
                    DriverName = "Suresh",
                    DriverContact = "9876543213",
                    IsActive = true,
                    SeatLayoutTemplateId = 1,
                    VendorId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Stops
        modelBuilder
            .Entity<Stop>()
            .HasData(
                new Stop
                {
                    StopId = 1,
                    Name = "Chennai Central",
                    Landmark = "Railway Station",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new Stop
                {
                    StopId = 2,
                    Name = "Electronic City",
                    Landmark = "IT Hub",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new Stop
                {
                    StopId = 3,
                    Name = "Trichy",
                    Landmark = "Central Bus Stand",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new Stop
                {
                    StopId = 4,
                    Name = "Mumbai Central",
                    Landmark = "Railway Station",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new Stop
                {
                    StopId = 5,
                    Name = "Pune Station",
                    Landmark = "Railway Station",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed RouteStops
        modelBuilder
            .Entity<RouteStop>()
            .HasData(
                // Chennai to Bangalore route stops
                new RouteStop
                {
                    RouteStopId = 1,
                    RouteId = 1,
                    StopId = 1, // Chennai Central
                    OrderNumber = 1,
                    DepartureTime = new TimeSpan(6, 0, 0),
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new RouteStop
                {
                    RouteStopId = 2,
                    RouteId = 1,
                    StopId = 3, // Trichy
                    OrderNumber = 2,
                    ArrivalTime = new TimeSpan(9, 0, 0),
                    DepartureTime = new TimeSpan(9, 15, 0),
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new RouteStop
                {
                    RouteStopId = 3,
                    RouteId = 1,
                    StopId = 2, // Electronic City (Bangalore)
                    OrderNumber = 3,
                    ArrivalTime = new TimeSpan(12, 0, 0),
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                // Mumbai to Pune route stops
                new RouteStop
                {
                    RouteStopId = 4,
                    RouteId = 2,
                    StopId = 4, // Mumbai Central
                    OrderNumber = 1,
                    DepartureTime = new TimeSpan(8, 0, 0),
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new RouteStop
                {
                    RouteStopId = 5,
                    RouteId = 2,
                    StopId = 5, // Pune Station
                    OrderNumber = 2,
                    ArrivalTime = new TimeSpan(11, 0, 0),
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Bus Schedules
        modelBuilder
            .Entity<BusSchedule>()
            .HasData(
                new BusSchedule
                {
                    ScheduleId = 1,
                    TravelDate = new DateTime(2025, 12, 25),
                    DepartureTime = new TimeSpan(6, 0, 0),
                    ArrivalTime = new TimeSpan(12, 0, 0),
                    Status = ScheduleStatus.Scheduled,
                    IsActive = true,
                    AvailableSeats = 38,
                    BusId = 1,
                    RouteId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new BusSchedule
                {
                    ScheduleId = 2,
                    TravelDate = new DateTime(2025, 12, 30),
                    DepartureTime = new TimeSpan(8, 0, 0),
                    ArrivalTime = new TimeSpan(14, 0, 0),
                    Status = ScheduleStatus.Scheduled,
                    IsActive = true,
                    AvailableSeats = 40,
                    BusId = 1,
                    RouteId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                },
                new BusSchedule
                {
                    ScheduleId = 3,
                    TravelDate = new DateTime(2026, 1, 15),
                    DepartureTime = new TimeSpan(10, 0, 0),
                    ArrivalTime = new TimeSpan(16, 0, 0),
                    Status = ScheduleStatus.Scheduled,
                    IsActive = true,
                    AvailableSeats = 40,
                    BusId = 1,
                    RouteId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Bookings
        modelBuilder
            .Entity<Booking>()
            .HasData(
                new Booking
                {
                    BookingId = 1,
                    PNRNo = "PNR12345",
                    CustomerId = 1,
                    TotalSeats = 2,
                    TotalAmount = 1200m,
                    TravelDate = new DateTime(2025, 12, 25),
                    Status = BookingStatus.Confirmed,
                    BookedAt = new DateTime(2024, 11, 1),
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 11, 1),
                }
            );

        // Seed Booking Segments
        modelBuilder
            .Entity<BookingSegment>()
            .HasData(
                new BookingSegment
                {
                    BookingSegmentId = 1,
                    BookingId = 1,
                    ScheduleId = 1,
                    SeatsBooked = 2,
                    SegmentAmount = 1000m,
                    SegmentOrder = 1,
                    BoardingStopId = 1,
                    DroppingStopId = 3,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Booked Seats
        modelBuilder
            .Entity<BookedSeat>()
            .HasData(
                new BookedSeat
                {
                    BookedSeatId = 1,
                    TravelDate = new DateTime(2025, 12, 25),
                    SeatNumber = "A1",
                    SeatType = SeatType.Seater,
                    SeatPosition = SeatPosition.Window,
                    PassengerName = "John Doe",
                    PassengerAge = 33,
                    PassengerGender = Gender.Male,
                    BookingId = 1,
                    BookingSegmentId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 11, 1),
                },
                new BookedSeat
                {
                    BookedSeatId = 2,
                    TravelDate = new DateTime(2025, 12, 25),
                    SeatNumber = "A2",
                    SeatType = SeatType.Seater,
                    SeatPosition = SeatPosition.Aisle,
                    PassengerName = "Jane Doe",
                    PassengerAge = 30,
                    PassengerGender = Gender.Female,
                    BookingId = 1,
                    BookingSegmentId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 11, 1),
                }
            );

        // Seed Payments
        modelBuilder
            .Entity<Payment>()
            .HasData(
                new Payment
                {
                    PaymentId = 1,
                    Amount = 1200m,
                    PaymentMethod = PaymentMethod.UPI,
                    PaymentStatus = PaymentStatus.Success,
                    PaymentDate = new DateTime(2024, 11, 1),
                    TransactionId = "TXN123456789",
                    IsActive = true,
                    BookingId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 11, 1),
                }
            );

        // Seed Vendor Documents
        modelBuilder
            .Entity<VendorDocument>()
            .HasData(
                new VendorDocument
                {
                    DocumentId = 1,
                    VendorId = 1,
                    DocumentFile = DocumentCategory.BusinessLicense,
                    DocumentPath = "/docs/license.pdf",
                    IssueDate = new DateTime(2023, 1, 1),
                    ExpiryDate = new DateTime(2028, 1, 1),
                    UploadedAt = new DateTime(2024, 1, 1),
                    IsVerified = true,
                    VerifiedAt = new DateTime(2024, 1, 1),
                    VerifiedBy = "Admin",
                    Status = DocumentStatus.Verified,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );

        // Seed Reviews
        modelBuilder
            .Entity<Review>()
            .HasData(
                new Review
                {
                    ReviewId = 1,
                    Rating = 5,
                    Comment = "Excellent service and comfortable journey!",
                    CustomerId = 1,
                    BusId = 1,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                }
            );
    }
}
