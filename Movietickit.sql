


create database dbmovietickit;

use dbmovietickit;

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- Store hashed passwords
    PhoneNumber NVARCHAR(15) UNIQUE
);

CREATE TABLE Theatres (
    TheatreID INT IDENTITY(1,1) PRIMARY KEY,
    TheatreName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(100) NOT NULL,
    TotalSeats INT NOT NULL CHECK (TotalSeats > 0)
);

CREATE TABLE Movies (
    MovieID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(50),
    Director NVARCHAR(50),
    Cast NVARCHAR(MAX), -- Using NVARCHAR(MAX) for large text
    Duration INT NOT NULL CHECK (Duration > 0),  -- Duration in minutes
    ReleaseDate DATE,
    Rating DECIMAL(3,1) CHECK (Rating BETWEEN 0 AND 10)
);

CREATE TABLE Showtimes (
    ShowtimeID INT IDENTITY(1,1) PRIMARY KEY,
    MovieID INT NOT NULL,
    TheatreID INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Date DATE NOT NULL,
    FOREIGN KEY (MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE,
    FOREIGN KEY (TheatreID) REFERENCES Theatres(TheatreID) ON DELETE CASCADE
);

CREATE TABLE Bookings (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ShowtimeID INT NOT NULL,
    BookingDate DATE NOT NULL DEFAULT GETDATE(),
    SeatsBooked INT NOT NULL CHECK (SeatsBooked > 0),
    TotalPrice DECIMAL(10,2) NOT NULL CHECK (TotalPrice >= 0),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ShowtimeID) REFERENCES Showtimes(ShowtimeID) ON DELETE CASCADE
);

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentDate DATE NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) NOT NULL CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed')),
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);

ALTER TABLE Payments DROP CONSTRAINT FK_Payments_Bookings;

select * from Admins;

alter table Movies add Price int;

-- Add columns to Bookings table
ALTER TABLE Bookings
ADD 
    SeatNumbers NVARCHAR(MAX) NOT NULL DEFAULT '',
    SeatTypes NVARCHAR(MAX) NOT NULL DEFAULT '';

-- Add columns to Payments table
ALTER TABLE Payments
ADD 
    Amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    PaymentDetails NVARCHAR(255) NULL,
    TransactionID NVARCHAR(50) NULL;

	CREATE TABLE Admins (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL, -- Store securely (Hash it)
    
    
    Role NVARCHAR(20) DEFAULT 'Admin',
    CreatedAt DATETIME DEFAULT GETDATE()
);


