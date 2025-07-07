-- USERS TABLE
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(15),
    Role NVARCHAR(20) CHECK (Role IN ('guest', 'user', 'vendor', 'admin')) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- VENDORS TABLE
CREATE TABLE Vendors (
    VendorID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    BusesManaged INT DEFAULT 0,
    Status NVARCHAR(20) CHECK (Status IN ('Active', 'Inactive')) NOT NULL
);

-- BUSES TABLE
CREATE TABLE Buses (
    BusID INT PRIMARY KEY IDENTITY(1,1),
    BusName NVARCHAR(100) NOT NULL,
    VendorID INT FOREIGN KEY REFERENCES Vendors(VendorID),
    Type NVARCHAR(50),
    RegistrationNo NVARCHAR(50) UNIQUE NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Active', 'Inactive')) NOT NULL
);

-- ROUTES TABLE
CREATE TABLE Routes (
    RouteID INT PRIMARY KEY IDENTITY(1,1),
    Source NVARCHAR(100) NOT NULL,
    Destination NVARCHAR(100) NOT NULL,
    Distance DECIMAL(6,2),
    Duration INT
);

-- BUS SCHEDULES TABLE
CREATE TABLE Bus_Schedules (
    ScheduleID INT PRIMARY KEY IDENTITY(1,1),
    BusID INT FOREIGN KEY REFERENCES Buses(BusID),
    RouteID INT FOREIGN KEY REFERENCES Routes(RouteID),
    TravelDate DATE NOT NULL,
    DepartureTime TIME NOT NULL,
    ArrivalTime TIME NOT NULL,
    Fare DECIMAL(10,2) NOT NULL
);

-- SEAT AVAILABILITY TABLE
CREATE TABLE Seat_Availability (
    BusID INT FOREIGN KEY REFERENCES Buses(BusID),
    TravelDate DATE,
    AvailableSeats INT CHECK (AvailableSeats >= 0),
    TotalSeats INT CHECK (TotalSeats > 0),
    PRIMARY KEY (BusID, TravelDate)
);

-- BOOKINGS TABLE
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    PNRNo NVARCHAR(20) UNIQUE NOT NULL,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    BusID INT FOREIGN KEY REFERENCES Buses(BusID),
    TravelDate DATE NOT NULL,
    SeatNumbers NVARCHAR(100),
    Status NVARCHAR(20) CHECK (Status IN ('Booked', 'Cancelled')) NOT NULL,
    PaymentID INT,
    BookedAt DATETIME DEFAULT GETDATE()
);

-- BOOKED SEATS TABLE
CREATE TABLE Booked_Seats (
    BookingSeatID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT FOREIGN KEY REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    BusID INT FOREIGN KEY REFERENCES Buses(BusID),
    TravelDate DATE NOT NULL,
    SeatNumber NVARCHAR(10) NOT NULL,
    UNIQUE (BusID, TravelDate, SeatNumber)
);

-- PAYMENTS TABLE
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT UNIQUE FOREIGN KEY REFERENCES Bookings(BookingID),
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(50),
    PaymentStatus NVARCHAR(20) CHECK (PaymentStatus IN ('Success', 'Failed', 'Pending')) NOT NULL
);

-- REVIEWS TABLE
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    BusID INT FOREIGN KEY REFERENCES Buses(BusID),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);
