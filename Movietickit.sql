


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


INSERT INTO Movies (Title, Genre, Director, Cast, Duration, ReleaseDate, Rating, ImageUrl, Price) VALUES
('Inception', 'Sci-Fi', 'Christopher Nolan', 'Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page', 148, '2010-07-16', 8.8, 'https://image.tmdb.org/t/p/w500/qmDpIHrmpJINaRKAfWQfftjCdyi.jpg', 1099),
('The Dark Knight', 'Action', 'Christopher Nolan', 'Christian Bale, Heath Ledger, Aaron Eckhart', 152, '2008-07-18', 9.0, 'https://image.tmdb.org/t/p/w500/1hRoyzDtpgMU7Dz4JF22RANzQO7.jpg', 1299),
('Interstellar', 'Sci-Fi', 'Christopher Nolan', 'Matthew McConaughey, Anne Hathaway, Jessica Chastain', 169, '2014-11-07', 8.7, 'https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg', 1199),
('The Shawshank Redemption', 'Drama', 'Frank Darabont', 'Tim Robbins, Morgan Freeman, Bob Gunton', 142, '1994-09-23', 9.3, 'https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg', 999),
('Pulp Fiction', 'Crime', 'Quentin Tarantino', 'John Travolta, Uma Thurman, Samuel L. Jackson', 154, '1994-10-14', 8.9, 'https://image.tmdb.org/t/p/w500/t3yQp4W0yNkwcNqN9NAwTrYfr3P.jpg', 1099),
('Forrest Gump', 'Drama', 'Robert Zemeckis', 'Tom Hanks, Robin Wright, Gary Sinise', 142, '1994-07-06', 8.8, 'https://image.tmdb.org/t/p/w500/h5J4W4veyxMXDMjeNxZI46TsHOb.jpg', 999),
('The Matrix', 'Sci-Fi', 'Lana Wachowski, Lilly Wachowski', 'Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss', 136, '1999-03-31', 8.7, 'https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg', 1199),
('Gladiator', 'Action', 'Ridley Scott', 'Russell Crowe, Joaquin Phoenix, Connie Nielsen', 155, '2000-05-05', 8.5, 'https://image.tmdb.org/t/p/w500/8cdbuLxutQjM2IRWnjFE3wT2Pek.jpg', 1099),
('Titanic', 'Romance', 'James Cameron', 'Leonardo DiCaprio, Kate Winslet, Billy Zane', 194, '1997-12-19', 7.9, 'https://image.tmdb.org/t/p/w500/kHXEpyfl6zqn8a6YuozZUujufXf.jpg', 899),
('The Godfather', 'Crime', 'Francis Ford Coppola', 'Marlon Brando, Al Pacino, James Caan', 175, '1972-03-24', 9.2, 'https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg', 1299),
('The Lion King', 'Animation', 'Roger Allers, Rob Minkoff', 'Matthew Broderick, Jeremy Irons, James Earl Jones', 88, '1994-06-24', 8.5, 'https://image.tmdb.org/t/p/w500/2NbddsyNCUjlpT5lV1sw1jgl9rB.jpg', 799),
('Joker', 'Drama', 'Todd Phillips', 'Joaquin Phoenix, Robert De Niro, Zazie Beetz', 122, '2019-10-04', 8.4, 'https://image.tmdb.org/t/p/w500/udDclJoHjfjb8Ekgsd4FDteOkCU.jpg', 1099),
('Spider-Man: No Way Home', 'Action', 'Jon Watts', 'Tom Holland, Zendaya, Benedict Cumberbatch', 148, '2021-12-17', 8.2, 'https://image.tmdb.org/t/p/w500/5XzjL79Jp4O64JdzCG3oyboOrin.jpg', 1199),
('Dune', 'Sci-Fi', 'Denis Villeneuve', 'Timothée Chalamet, Rebecca Ferguson, Zendaya', 155, '2021-10-22', 8.0, 'https://image.tmdb.org/t/p/w500/d5NXSklXo0qyIYkgV94XAgMIckC.jpg', 1099),
('The Batman', 'Action', 'Matt Reeves', 'Robert Pattinson, Zoë Kravitz, Paul Dano', 176, '2022-03-04', 7.9, 'https://image.tmdb.org/t/p/w500/74xTEgt7R36Fpooo50r9T25onhq.jpg', 1199),
('Oppenheimer', 'Biography', 'Christopher Nolan', 'Cillian Murphy, Emily Blunt, Robert Downey Jr.', 180, '2023-07-21', 8.6, 'https://image.tmdb.org/t/p/w500/vL5LR6WdxWPjLPFRLe133jXWsh5.jpg', 1399),
('Everything Everywhere All at Once', 'Adventure', 'Daniel Kwan, Daniel Scheinert', 'Michelle Yeoh, Stephanie Hsu, Ke Huy Quan', 139, '2022-03-11', 8.1, 'https://image.tmdb.org/t/p/w500/7t4j0WE5wHpp7X3mRAnv3xGmMEr.jpg', 999),
('John Wick: Chapter 4', 'Action', 'Chad Stahelski', 'Keanu Reeves, Donnie Yen, Bill Skarsgård', 169, '2023-03-24', 7.8, 'https://image.tmdb.org/t/p/w500/vZloFAK7NmvMGKE7VkF5UHaz0I.jpg', 1099),
('Guardians of the Galaxy Vol. 3', 'Action', 'James Gunn', 'Chris Pratt, Zoe Saldana, Dave Bautista', 150, '2023-05-05', 8.1, 'https://image.tmdb.org/t/p/w500/r2J02Z2OpNTctfOSN1Ydgii51I3.jpg', 1199);

