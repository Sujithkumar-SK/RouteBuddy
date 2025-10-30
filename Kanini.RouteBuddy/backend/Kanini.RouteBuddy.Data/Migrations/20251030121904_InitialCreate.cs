using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanini.RouteBuddy.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Distance = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TIME", nullable: false),
                    BasePrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                },
                comment: "Route details and information");

            migrationBuilder.CreateTable(
                name: "SeatLayoutTemplates",
                columns: table => new
                {
                    SeatLayoutTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Template name like 'AC Seater 2x2', 'Sleeper 2x1'"),
                    TotalSeats = table.Column<int>(type: "INT", nullable: false),
                    BusType = table.Column<int>(type: "INT", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatLayoutTemplates", x => x.SeatLayoutTemplateId);
                },
                comment: "Master templates for different bus seat layouts");

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    StopId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    Landmark = table.Column<string>(type: "NVARCHAR(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.StopId);
                },
                comment: "Route stops and landmarks information");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Last successful login timestamp"),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false, comment: "Number of failed login attempts"),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Account lockout until this time"),
                    Phone = table.Column<string>(type: "NVARCHAR(10)", nullable: false),
                    Role = table.Column<int>(type: "INT", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "BIT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                },
                comment: "Stores user authentication and login information");

            migrationBuilder.CreateTable(
                name: "SeatLayoutDetails",
                columns: table => new
                {
                    SeatLayoutDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatLayoutTemplateId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: false, comment: "Seat identifier like 'A1', 'B2U', 'C3L'"),
                    SeatType = table.Column<int>(type: "INT", nullable: false, comment: "Type of seat - Seater, SleeperUpper, SleeperLower, etc."),
                    SeatPosition = table.Column<int>(type: "INT", nullable: false, comment: "Position of seat - Window, Aisle, Middle"),
                    RowNumber = table.Column<int>(type: "INT", nullable: false, comment: "Physical row number in the bus"),
                    ColumnNumber = table.Column<int>(type: "INT", nullable: false, comment: "Position within the row (1, 2, 3, 4)"),
                    PriceTier = table.Column<int>(type: "INT", nullable: false, comment: "Pricing tier for this seat - Base, Premium, Luxury"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatLayoutDetails", x => x.SeatLayoutDetailId);
                    table.ForeignKey(
                        name: "FK_SeatLayoutDetails_SeatLayoutTemplates_SeatLayoutTemplateId",
                        column: x => x.SeatLayoutTemplateId,
                        principalTable: "SeatLayoutTemplates",
                        principalColumn: "SeatLayoutTemplateId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Individual seat definitions within seat layout templates");

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    RouteStopId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    StopId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false, comment: "Defines the order of the stop in the route"),
                    ArrivalTime = table.Column<TimeSpan>(type: "TIME", nullable: true),
                    DepartureTime = table.Column<TimeSpan>(type: "TIME", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.RouteStopId);
                    table.ForeignKey(
                        name: "FK_RouteStops_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteStops_Stops_StopId",
                        column: x => x.StopId,
                        principalTable: "Stops",
                        principalColumn: "StopId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Junction table linking routes and stops with order and timing");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Customer's first name"),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Customer's middle name"),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Customer's last name"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Customer's date of birth"),
                    Gender = table.Column<int>(type: "INT", nullable: false),
                    ProfilePicture = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyName = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    OwnerName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    BusinessLicenseNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    OfficeAddress = table.Column<string>(type: "NVARCHAR(300)", maxLength: 300, nullable: false),
                    FleetSize = table.Column<int>(type: "INT", nullable: false),
                    TaxRegistrationNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorId);
                    table.ForeignKey(
                        name: "FK_Vendors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Vendor information");

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PNRNo = table.Column<string>(type: "NVARCHAR(12)", maxLength: 12, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TotalSeats = table.Column<int>(type: "INT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    TravelDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    BookedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Customer booking details and management");

            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    BusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    BusType = table.Column<int>(type: "INT", nullable: false),
                    TotalSeats = table.Column<int>(type: "INT", nullable: false),
                    RegistrationNo = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    RegistrationPath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    Amenities = table.Column<int>(type: "INT", nullable: false),
                    DriverName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    DriverContact = table.Column<string>(type: "NVARCHAR(10)", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    SeatLayoutTemplateId = table.Column<int>(type: "int", nullable: true, comment: "Optional seat layout template for this bus"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.BusId);
                    table.ForeignKey(
                        name: "FK_Buses_SeatLayoutTemplates_SeatLayoutTemplateId",
                        column: x => x.SeatLayoutTemplateId,
                        principalTable: "SeatLayoutTemplates",
                        principalColumn: "SeatLayoutTemplateId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Buses_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bus details and information");

            migrationBuilder.CreateTable(
                name: "VendorDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    DocumentFile = table.Column<int>(type: "INT", nullable: false),
                    DocumentPath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Document issue date"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Document expiry date"),
                    UploadedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    IsVerified = table.Column<bool>(type: "BIT", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    RejectedReason = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_VendorDocuments_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Vendor document verification and management");

            migrationBuilder.CreateTable(
                name: "Cancellations",
                columns: table => new
                {
                    CancellationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CancelledOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CancelledBy = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "NVARCHAR(250)", maxLength: 250, nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellations", x => x.CancellationId);
                    table.ForeignKey(
                        name: "FK_Cancellations_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Booking cancellation details and penalty information");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INT", nullable: false),
                    PaymentStatus = table.Column<int>(type: "INT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    TransactionId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Payment processing and transaction details");

            migrationBuilder.CreateTable(
                name: "BusPhotos",
                columns: table => new
                {
                    BusPhotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "NVARCHAR(500)", nullable: true),
                    Caption = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusPhotos", x => x.BusPhotoId);
                    table.ForeignKey(
                        name: "FK_BusPhotos_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bus images and photo gallery");

            migrationBuilder.CreateTable(
                name: "BusSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    DepartureTime = table.Column<TimeSpan>(type: "TIME", nullable: false),
                    ArrivalTime = table.Column<TimeSpan>(type: "TIME", nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    AvailableSeats = table.Column<int>(type: "INT", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_BusSchedules_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusSchedules_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Bus schedule and timing information");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "INT", nullable: false),
                    Comment = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Customer reviews and ratings for buses");

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    RefundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    RefundMethod = table.Column<int>(type: "INT", nullable: false),
                    RefundStatus = table.Column<int>(type: "INT", nullable: false),
                    RefundedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_Refunds_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Payment refund processing and status tracking");

            migrationBuilder.CreateTable(
                name: "BookingSegments",
                columns: table => new
                {
                    BookingSegmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    SeatsBooked = table.Column<int>(type: "INT", nullable: false),
                    SegmentAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    SegmentOrder = table.Column<int>(type: "INT", nullable: false),
                    BoardingStopId = table.Column<int>(type: "int", nullable: true, comment: "Boarding stop for this segment"),
                    DroppingStopId = table.Column<int>(type: "int", nullable: true, comment: "Dropping stop for this segment"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSegments", x => x.BookingSegmentId);
                    table.ForeignKey(
                        name: "FK_BookingSegments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingSegments_BusSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "BusSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingSegments_RouteStops_BoardingStopId",
                        column: x => x.BoardingStopId,
                        principalTable: "RouteStops",
                        principalColumn: "RouteStopId");
                    table.ForeignKey(
                        name: "FK_BookingSegments_RouteStops_DroppingStopId",
                        column: x => x.DroppingStopId,
                        principalTable: "RouteStops",
                        principalColumn: "RouteStopId");
                },
                comment: "Segments of a booking associated with specific bus schedules");

            migrationBuilder.CreateTable(
                name: "BookedSeats",
                columns: table => new
                {
                    BookedSeatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    SeatNumber = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: false),
                    SeatType = table.Column<int>(type: "INT", nullable: false),
                    SeatPosition = table.Column<int>(type: "INT", nullable: false),
                    PassengerName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    PassengerAge = table.Column<int>(type: "INT", nullable: false),
                    PassengerGender = table.Column<int>(type: "INT", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    BookingSegmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedSeats", x => x.BookedSeatId);
                    table.ForeignKey(
                        name: "FK_BookedSeats_BookingSegments_BookingSegmentId",
                        column: x => x.BookingSegmentId,
                        principalTable: "BookingSegments",
                        principalColumn: "BookingSegmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookedSeats_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                },
                comment: "Individual seat bookings with passenger details");

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "RouteId", "BasePrice", "CreatedBy", "CreatedOn", "Destination", "Distance", "Duration", "IsActive", "Source", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, 500m, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bangalore", 350.5m, new TimeSpan(0, 6, 0, 0, 0), true, "Chennai", null, null },
                    { 2, 300m, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pune", 150.0m, new TimeSpan(0, 3, 0, 0, 0), true, "Mumbai", null, null }
                });

            migrationBuilder.InsertData(
                table: "SeatLayoutTemplates",
                columns: new[] { "SeatLayoutTemplateId", "BusType", "CreatedBy", "CreatedOn", "Description", "IsActive", "TemplateName", "TotalSeats", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Standard AC bus with 2x2 seating arrangement", true, "AC Seater 2x2", 40, null, null },
                    { 2, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sleeper bus with upper and lower berths", true, "Sleeper 2x1", 36, null, null }
                });

            migrationBuilder.InsertData(
                table: "Stops",
                columns: new[] { "StopId", "CreatedBy", "CreatedOn", "IsActive", "Landmark", "Name", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Railway Station", "Chennai Central", null, null },
                    { 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "IT Hub", "Electronic City", null, null },
                    { 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Central Bus Stand", "Trichy", null, null },
                    { 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Railway Station", "Mumbai Central", null, null },
                    { 5, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Railway Station", "Pune Station", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedBy", "CreatedOn", "Email", "FailedLoginAttempts", "IsActive", "IsEmailVerified", "LastLogin", "LockoutEnd", "PasswordHash", "Phone", "Role", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@routebuddy.com", 0, true, true, null, null, "hashedpassword123", "9876543210", 3, null, null },
                    { 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer1@gmail.com", 0, true, true, null, null, "hashedpassword123", "9876543211", 1, null, null },
                    { 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "vendor1@gmail.com", 0, true, true, null, null, "hashedpassword123", "9876543212", 2, null, null }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedBy", "CreatedOn", "DateOfBirth", "FirstName", "Gender", "IsActive", "LastName", "MiddleName", "ProfilePicture", "UpdatedBy", "UpdatedOn", "UserId" },
                values: new object[] { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", 1, true, "Doe", null, null, null, null, 2 });

            migrationBuilder.InsertData(
                table: "RouteStops",
                columns: new[] { "RouteStopId", "ArrivalTime", "CreatedBy", "CreatedOn", "DepartureTime", "OrderNumber", "RouteId", "StopId", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 6, 0, 0, 0), 1, 1, 1, null, null },
                    { 2, new TimeSpan(0, 9, 0, 0, 0), "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 9, 15, 0, 0), 2, 1, 3, null, null },
                    { 3, new TimeSpan(0, 12, 0, 0, 0), "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, 1, 2, null, null },
                    { 4, null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0), 1, 2, 4, null, null },
                    { 5, new TimeSpan(0, 11, 0, 0, 0), "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 2, 5, null, null }
                });

            migrationBuilder.InsertData(
                table: "SeatLayoutDetails",
                columns: new[] { "SeatLayoutDetailId", "ColumnNumber", "CreatedBy", "CreatedOn", "PriceTier", "RowNumber", "SeatLayoutTemplateId", "SeatNumber", "SeatPosition", "SeatType", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1, "A1", 1, 1, null, null },
                    { 2, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, "A2", 2, 1, null, null },
                    { 3, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, "A3", 2, 1, null, null },
                    { 4, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1, "A4", 1, 1, null, null },
                    { 5, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 1, "B1", 1, 1, null, null },
                    { 6, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, "B2", 2, 1, null, null },
                    { 7, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, "B3", 2, 1, null, null },
                    { 8, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 1, "B4", 1, 1, null, null },
                    { 9, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, 1, "C1", 1, 1, null, null },
                    { 10, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 1, "C2", 2, 1, null, null },
                    { 11, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 1, "C3", 2, 1, null, null },
                    { 12, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, 1, "C4", 1, 1, null, null },
                    { 13, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4, 1, "D1", 1, 1, null, null },
                    { 14, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 1, "D2", 2, 1, null, null },
                    { 15, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 1, "D3", 2, 1, null, null },
                    { 16, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4, 1, "D4", 1, 1, null, null },
                    { 17, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 1, "E1", 1, 1, null, null },
                    { 18, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 1, "E2", 2, 1, null, null },
                    { 19, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 1, "E3", 2, 1, null, null },
                    { 20, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 1, "E4", 1, 1, null, null },
                    { 21, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 1, "F1", 1, 1, null, null },
                    { 22, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6, 1, "F2", 2, 1, null, null },
                    { 23, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6, 1, "F3", 2, 1, null, null },
                    { 24, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 1, "F4", 1, 1, null, null },
                    { 25, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7, 1, "G1", 1, 1, null, null },
                    { 26, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, 1, "G2", 2, 1, null, null },
                    { 27, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, 1, "G3", 2, 1, null, null },
                    { 28, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7, 1, "G4", 1, 1, null, null },
                    { 29, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "H1", 1, 1, null, null },
                    { 30, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8, 1, "H2", 2, 1, null, null },
                    { 31, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8, 1, "H3", 2, 1, null, null },
                    { 32, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "H4", 1, 1, null, null },
                    { 33, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9, 1, "I1", 1, 1, null, null },
                    { 34, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 9, 1, "I2", 2, 1, null, null },
                    { 35, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 9, 1, "I3", 2, 1, null, null },
                    { 36, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9, 1, "I4", 1, 1, null, null },
                    { 37, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "J1", 1, 1, null, null },
                    { 38, 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 1, "J2", 2, 1, null, null },
                    { 39, 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 1, "J3", 2, 1, null, null },
                    { 40, 4, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "J4", 1, 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "Vendors",
                columns: new[] { "VendorId", "AgencyName", "BusinessLicenseNumber", "CreatedBy", "CreatedOn", "FleetSize", "IsActive", "OfficeAddress", "OwnerName", "Status", "TaxRegistrationNumber", "UpdatedBy", "UpdatedOn", "UserId" },
                values: new object[] { 1, "Express Travels", "BL123456", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, true, "123 Main St, Chennai", "Rajesh Kumar", 1, "TAX789012", null, null, 3 });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookedAt", "CancelledAt", "CreatedBy", "CreatedOn", "CustomerId", "IsActive", "PNRNo", "Status", "TotalAmount", "TotalSeats", "TravelDate", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "System", new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "PNR12345", 2, 1200m, 2, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "BusId", "Amenities", "BusName", "BusType", "CreatedBy", "CreatedOn", "DriverContact", "DriverName", "IsActive", "RegistrationNo", "RegistrationPath", "SeatLayoutTemplateId", "Status", "TotalSeats", "UpdatedBy", "UpdatedOn", "VendorId" },
                values: new object[] { 1, 3, "Express Deluxe", 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9876543213", "Suresh", true, "TN01AB1234", "/docs/reg1.pdf", 1, 1, 40, null, null, 1 });

            migrationBuilder.InsertData(
                table: "VendorDocuments",
                columns: new[] { "DocumentId", "CreatedBy", "CreatedOn", "DocumentFile", "DocumentPath", "ExpiryDate", "IsVerified", "IssueDate", "RejectedReason", "Status", "UpdatedBy", "UpdatedOn", "UploadedAt", "VendorId", "VerifiedAt", "VerifiedBy" },
                values: new object[] { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "/docs/license.pdf", new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin" });

            migrationBuilder.InsertData(
                table: "BusSchedules",
                columns: new[] { "ScheduleId", "ArrivalTime", "AvailableSeats", "BusId", "CreatedBy", "CreatedOn", "DepartureTime", "IsActive", "RouteId", "Status", "TravelDate", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 12, 0, 0, 0), 38, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 6, 0, 0, 0), true, 1, 1, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, new TimeSpan(0, 14, 0, 0, 0), 40, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0), true, 1, 1, new DateTime(2025, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, new TimeSpan(0, 16, 0, 0, 0), 40, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 0, 0, 0), true, 1, 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "Amount", "BookingId", "CreatedBy", "CreatedOn", "IsActive", "PaymentDate", "PaymentMethod", "PaymentStatus", "TransactionId", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, 1200m, 1, "System", new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "TXN123456789", null, null });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "BusId", "Comment", "CreatedBy", "CreatedOn", "CustomerId", "Rating", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, 1, "Excellent service and comfortable journey!", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, null, null });

            migrationBuilder.InsertData(
                table: "BookingSegments",
                columns: new[] { "BookingSegmentId", "BoardingStopId", "BookingId", "CreatedBy", "CreatedOn", "DroppingStopId", "ScheduleId", "SeatsBooked", "SegmentAmount", "SegmentOrder", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, 1, 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, 2, 1000m, 1, null, null });

            migrationBuilder.InsertData(
                table: "BookedSeats",
                columns: new[] { "BookedSeatId", "BookingId", "BookingSegmentId", "CreatedBy", "CreatedOn", "PassengerAge", "PassengerGender", "PassengerName", "SeatNumber", "SeatPosition", "SeatType", "TravelDate", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, 1, 1, "System", new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 33, 1, "John Doe", "A1", 1, 1, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, 1, 1, "System", new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 2, "Jane Doe", "A2", 2, 1, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedSeats_BookingId",
                table: "BookedSeats",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookedSeats_BookingSegmentId",
                table: "BookedSeats",
                column: "BookingSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PNRNo",
                table: "Bookings",
                column: "PNRNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PNRNo",
                table: "Bookings",
                column: "PNRNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSegments_BoardingStopId",
                table: "BookingSegments",
                column: "BoardingStopId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSegments_BookingId",
                table: "BookingSegments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSegments_DroppingStopId",
                table: "BookingSegments",
                column: "DroppingStopId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSegments_ScheduleId",
                table: "BookingSegments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_RegistrationNo",
                table: "Buses",
                column: "RegistrationNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buses_SeatLayoutTemplateId",
                table: "Buses",
                column: "SeatLayoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_VendorId",
                table: "Buses",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationNo",
                table: "Buses",
                column: "RegistrationNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusPhotos_BusId",
                table: "BusPhotos",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusSchedules_BusId",
                table: "BusSchedules",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusSchedules_RouteId",
                table: "BusSchedules",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_BookingId",
                table: "Cancellations",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId1",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusId",
                table: "Reviews",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_OrderNumber",
                table: "RouteStops",
                columns: new[] { "RouteId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_StopId",
                table: "RouteStops",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatLayoutDetails_SeatLayoutTemplateId",
                table: "SeatLayoutDetails",
                column: "SeatLayoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorDocuments_VendorId",
                table: "VendorDocuments",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLicenseNumber",
                table: "Vendors",
                column: "BusinessLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRegistrationNumber",
                table: "Vendors",
                column: "TaxRegistrationNumber",
                unique: true,
                filter: "[TaxRegistrationNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessLicenseNumber",
                table: "Vendors",
                column: "BusinessLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_TaxRegistrationNumber",
                table: "Vendors",
                column: "TaxRegistrationNumber",
                unique: true,
                filter: "[TaxRegistrationNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedSeats");

            migrationBuilder.DropTable(
                name: "BusPhotos");

            migrationBuilder.DropTable(
                name: "Cancellations");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SeatLayoutDetails");

            migrationBuilder.DropTable(
                name: "VendorDocuments");

            migrationBuilder.DropTable(
                name: "BookingSegments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "BusSchedules");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Stops");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SeatLayoutTemplates");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
