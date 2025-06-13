using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onlinemovietickitproject.Models;
using System.Web.Security;
using System.Data.Entity;
using PagedList;
using System.Data.Entity.Validation;
using System.Text;
using QRCoder;
using System.Drawing;
using System.IO;

namespace Onlinemovietickitproject.Controllers
{
    [HandleError]
    public class UserController : Controller
    {
        private readonly dbmovietickitEntities1 db = new dbmovietickitEntities1();



        // GET: User
        public ActionResult Index()
        {
            Movieupcomingdata movieData = new Movieupcomingdata();
            List<Movieapi> movies = movieData.GetMovies();

            // Optionally: Store movies in ViewBag, ViewModel, or TempData
            ViewBag.UpcomingMovies = movies;

            // Return the login view (you can access movies via ViewBag.UpcomingMovies)
            return View();

        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Try to authenticate as Admin
                var admin = db.Admins.FirstOrDefault(a =>
                    (a.Username == model.EmailOrUsername || a.Email == model.EmailOrUsername)
                    && a.Password == model.Password);

                if (admin != null)
                {
                    FormsAuthentication.SetAuthCookie(admin.Username, model.RememberMe);
                    Session["AdminID"] = admin.AdminID;
                    Session["Role"] = "Admin";
                    return RedirectToAction("Index", "Admin");  // Admin controller
                }

                // Try to authenticate as User
                var user = db.Users.FirstOrDefault(u =>
                    (u.Username == model.EmailOrUsername || u.Email == model.EmailOrUsername)
                    && u.Password == model.Password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, model.RememberMe);
                    Session["UserID"] = user.UserID;
                    Session["Role"] = "User";
                    return RedirectToAction("Index", "User");  // User controller
                }

                ViewBag.Error = "Invalid username/email or password";
            }

            return View(model);
        }




        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User userModel)
        {
            // Check if the user with same email or username already exists
            var existingUser = db.Users.FirstOrDefault(u => u.Email == userModel.Email || u.Username == userModel.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "User with this email or username is already registered.");
                return View(userModel);
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(userModel);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(userModel);
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            if (Request.Cookies["UserID"] != null)
            {
                var cookie = new HttpCookie("UserID");

                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Login", "User");
        }



        public ActionResult Movie(int? page, string selectedGenres, string selectedYears, string selectedRatings)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var movies = db.Movies.AsQueryable();

            // Convert CSV string back to array
            var genreList = !string.IsNullOrEmpty(selectedGenres) ? selectedGenres.Split(',') : new string[0];
            var yearList = !string.IsNullOrEmpty(selectedYears) ? selectedYears.Split(',') : new string[0];
            var ratingList = !string.IsNullOrEmpty(selectedRatings) ? selectedRatings.Split(',') : new string[0];

            // Apply Genre Filter
            if (genreList.Length > 0)
            {
                movies = movies.Where(m => genreList.Contains(m.Genre));
            }

            // Apply Release Year Filter
            if (yearList.Length > 0)
            {
                var years = yearList.Select(y => int.TryParse(y, out int year) ? year : (int?)null)
                                    .Where(y => y.HasValue)
                                    .Select(y => y.Value)
                                    .ToList();

                movies = movies.Where(m => years.Contains(m.ReleaseDate.Value.Year));
            }

            // Apply Rating Filter
            if (ratingList.Length > 0)
            {
                var minRatings = ratingList.Select(r => decimal.TryParse(r, out decimal rating) ? rating : (decimal?)null)
                                           .Where(r => r.HasValue)
                                           .Select(r => r.Value)
                                           .ToList();

                movies = movies.Where(m => minRatings.Any(r => m.Rating >= r));
            }

            // Paginate results
            User user = new User
            {
                Moviecatagory = movies.OrderBy(m => m.Title).ToPagedList(pageNumber, pageSize)
            };

            return View(user);
        }



        public ActionResult Datails(int movieId)
        {
            // ✅ Login validation


            // ✅ Fetch movie data
            var movieData = db.Movies
                .Where(m => m.MovieID == movieId)
                .Select(m => new
                {
                    m.MovieID,
                    m.Title,
                    m.ImageUrl,
                    m.Genre,
                    m.Rating,
                    m.Duration,
                    m.ReleaseDate,
                    m.Price,
                    m.Cast,
                    m.Director
                })
                .FirstOrDefault();

            if (movieData == null)
            {
                return HttpNotFound();
            }

            // ✅ Map data to model
            var movie = new Movy
            {
                MovieID = movieData.MovieID,
                Title = movieData.Title,
                ImageUrl = movieData.ImageUrl,
                Genre = movieData.Genre,
                Rating = movieData.Rating,
                Duration = movieData.Duration,
                ReleaseDate = movieData.ReleaseDate,
                Price = movieData.Price,
                Cast = movieData.Cast,
                Director = movieData.Director
            };

            User user = new User
            {
                Moviedata = new List<Movy> { movie }
            };

            return View(user);
        }

        public ActionResult Theatre(int movieId)
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // ✅ First, retrieve the movie entity
            var movieEntity = db.Movies.FirstOrDefault(m => m.MovieID == movieId);

            if (movieEntity == null)
            {
                return HttpNotFound(); // ❌ Handles the case where the movie doesn't exist
            }

            // ✅ Manually map the entity to a new `Movy` object
            Movy movie = new Movy
            {
                MovieID = movieEntity.MovieID,
                Title = movieEntity.Title,
                Genre = movieEntity.Genre
            };

            var showtime = db.Showtimes.Where(s => s.MovieID == movieId).Join(db.Theatres, s => s.TheatreID, t => t.TheatreID, (s, t) => new
            {
                t.TheatreID,
                t.TheatreName,
                t.Location,
                s.ShowtimeID,
                s.StartTime,
                s.EndTime,
                s.Date
            }).ToList();




            // ✅ Ensure `Moviedata` is always initialized
            User user = new User
            {
                Moviedata = new List<Movy> { movie },

                TheatreData = showtime.Select(st => new Theatre
                {
                    TheatreID = st.TheatreID,
                    TheatreName = st.TheatreName,
                    Location = st.Location
                }).ToList(),

                showtimesData = showtime.Select(st => new Showtime
                {
                    ShowtimeID = st.ShowtimeID,
                    MovieID = movieId,
                    TheatreID = st.TheatreID,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime,
                    Date = st.Date
                }).ToList()
            };

            return View(user);
        }

        public ActionResult Movie_Tickit(int theatreId, string theatreName, string startTime, string movieTitle, int showtimeId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                if (theatreId == 0 || showtimeId == 0)
                {
                    return RedirectToAction("Index", "Error", new { message = "Invalid Theatre or Showtime ID" });
                }

                var theatre = db.Theatres.FirstOrDefault(t => t.TheatreID == theatreId);
                if (theatre == null)
                {
                    return RedirectToAction("Index", "Error", new { message = "Theatre not found" });
                }

                var movie = db.Movies.FirstOrDefault(m => m.Title == movieTitle);
                if (movie == null)
                {
                    return RedirectToAction("Index", "Error", new { message = "Movie not found" });
                }

                var showtime = db.Showtimes.Find(showtimeId);
                if (showtime == null)
                {
                    return RedirectToAction("Index", "Error", new { message = "Showtime not found" });
                }

                int totalSeats = theatre.TotalSeats;

                // Get booked seat numbers for the showtime and convert to list of integers
                var bookedSeatStrings = db.Bookings
                    .Where(b => b.ShowtimeID == showtimeId)
                    .Select(b => b.SeatNumbers)
                    .ToList();

                List<int> bookedSeats = bookedSeatStrings
                    .Where(s => !string.IsNullOrEmpty(s))
                    .SelectMany(s => s.Split(','))
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();

                List<int> availableSeats = Enumerable.Range(1, totalSeats)
                                                     .Where(seat => !bookedSeats.Contains(seat))
                                                     .ToList();

                // Pass data to view
                ViewBag.TotalSeats = totalSeats;
                ViewBag.Theatername = theatreName;
                ViewBag.StartTime = startTime;
                ViewBag.MovieTitle = movieTitle;
                ViewBag.BasePrice = movie.Price ?? 200;
                ViewBag.ShowtimeID = showtimeId;
                ViewBag.AvailableSeats = availableSeats;
                ViewBag.BookedSeats = bookedSeats;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        // Constants
        private const decimal BasePrice = 1000m;
        private const decimal VipMultiplier = 1.10m;
        private const decimal ConvenienceFeeRate = 0.10m;
        private const decimal TaxRate = 0.05m;

        // Show Payment Page
        public ActionResult Movie_Payment(
            List<string> selectedSeats,
            List<bool> isVip,
            string movieTitle,
            string theaterName,
            int showtimeId,
            string startTime,
            decimal totalAmount,
            decimal grandTotal)
        {
            try
            {
                selectedSeats = selectedSeats ?? new List<string>();
                isVip = isVip ?? new List<bool>();

                if (selectedSeats.Count != isVip.Count)
                    return RedirectToAction("Error", new { message = "Seat count and VIP flags mismatch" });

                if (selectedSeats.Count == 0)
                    return RedirectToAction("Error", new { message = "No seats selected" });

                var model = new PaymentViewModel
                {
                    SelectedSeats = selectedSeats,
                    IsVip = isVip,
                    MovieTitle = movieTitle,
                    TheaterName = theaterName,
                    ShowtimeID = showtimeId,
                    StartTime = DateTime.Parse(startTime),
                    TotalAmount = totalAmount
                };

                model.ConvenienceFee = Math.Round(model.TotalAmount * ConvenienceFeeRate, 2);
                model.Tax = Math.Round(model.TotalAmount * TaxRate, 2);
                model.GrandTotal = model.TotalAmount + model.ConvenienceFee + model.Tax;

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }

        // Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Movie_Payment", model);
            }

            try
            {
                if (Session["UserId"] == null)
                {
                    ModelState.AddModelError("", "User session expired. Please login again.");
                    return View("Movie_Payment", model);
                }

                int userId = (int)Session["UserId"];

                // Enhanced showtime validation
                var showtime = db.Showtimes
                    .Include(s => s.Movy)
                    .Include(s => s.Theatre)
                    .FirstOrDefault(s => s.ShowtimeID == model.ShowtimeID);

                if (showtime == null)
                {
                    ModelState.AddModelError("", $"Showtime not found for ID: {model.ShowtimeID}");
                    return View("Movie_Payment", model);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Create booking
                        var booking = new Booking
                        {
                            UserID = userId,
                            ShowtimeID = model.ShowtimeID,
                            BookingDate = DateTime.Now,
                            SeatsBooked = model.SelectedSeats.Count,
                            TotalPrice = model.GrandTotal,
                            SeatNumbers = string.Join(",", model.SelectedSeats),
                            SeatTypes = string.Join(",", model.IsVip.Select(vip => vip ? "VIP" : "Standard")),
                            StartTime = model.StartTime.TimeOfDay
                        };

                        db.Bookings.Add(booking);
                        db.SaveChanges();

                        // Create payment
                        var payment = new Payment
                        {
                            BookingID = booking.BookingID,
                            PaymentMethod = model.PaymentMethod,
                            PaymentDate = DateTime.Now,
                            PaymentStatus = "Completed",
                            Amount = model.GrandTotal,
                            PaymentDetails = GetPaymentDetails(model),
                            TransactionID = GenerateTransactionId()
                        };

                        db.Payments.Add(payment);
                        db.SaveChanges();

                        transaction.Commit();

                        return RedirectToAction("BookingConfirmation", new { bookingId = booking.BookingID });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", $"Error processing payment: {ex.Message}");
                        return View("Movie_Payment", model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View("Movie_Payment", model);
            }
        }



        // Helper Method: Payment Details
        private string GetPaymentDetails(PaymentViewModel model)
        {
            var details = new StringBuilder();
            details.AppendLine($"Payment Method: {model.PaymentMethod}");

            if (model.PaymentMethod == "Card")
            {
                details.AppendLine($"Card: **** **** **** {model.CardNumber?.Substring(Math.Max(0, model.CardNumber.Length - 4))}");
                details.AppendLine($"Cardholder: {model.CardName}");
            }
            else if (model.PaymentMethod == "UPI")
            {
                details.AppendLine($"UPI ID: {model.UpiId}");
            }
            else if (model.PaymentMethod == "Netbanking")
            {
                details.AppendLine($"Bank Code: {model.BankCode}");
            }

            return details.ToString();
        }

        // Helper Method: Generate Transaction ID
        private string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }

        // Booking Confirmation Page
        public ActionResult BookingConfirmation(int bookingId)
        {
            if (bookingId == 0)
                return RedirectToAction("Error", new { message = "Invalid booking ID" });

            // Fix the include paths to match your actual model
            var booking = db.Bookings
                .Include(b => b.Showtime.Movy)  // Changed from .Include("Showtime.Movie")
                .Include(b => b.Showtime.Theatre)
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
                return RedirectToAction("Error", new { message = "Booking not found" });

            var payment = db.Payments.FirstOrDefault(p => p.BookingID == bookingId);

            var model = new BookingConfirmationViewModel
            {
                BookingID = booking.BookingID,
                MovieTitle = booking.Showtime.Movy.Title,  // Changed from Movie to Movy
                TheaterName = booking.Showtime.Theatre.TheatreName,
                StartTime = booking.StartTime,
                SeatsBooked = booking.SeatNumbers?.Split(',').ToList() ?? new List<string>(),
                SeatTypes = booking.SeatTypes?.Split(',').ToList() ?? new List<string>(),
                TotalPrice = booking.TotalPrice,
                PaymentStatus = payment?.PaymentStatus ?? "Pending",
                TransactionID = payment?.TransactionID ?? "N/A",
                BookingDate = booking.BookingDate
            };

            return View(model);
        }


        public ActionResult User_payment_detail()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            var userPayments = db.Bookings
                .Where(b => b.UserID == userId)
                .SelectMany(b => b.Payments.Select(p => new BookingConfirmationViewModel
                {
                    Booking = b,
                    Payment = p,
                     TheatreName = b.Showtime.Theatre.TheatreName,
                    MovieTitle = b.Showtime.Movy.Title
                }))
                .ToList();

            return View(userPayments);
        }

        public ActionResult GenerateQRCode(string transactionId)
        {
            var payment = db.Payments
                .Include("Booking.Showtime.Movy")
                .Include("Booking.Showtime.Theatre")
                .FirstOrDefault(p => p.TransactionID == transactionId);

            if (payment == null)
            {
                return HttpNotFound("Payment not found");
            }

            var booking = payment.Booking;
            var showtime = booking.Showtime;
            var movie = showtime.Movy;
            var theatre = showtime.Theatre;

            // Create a summary string for the QR code
            string qrData = $"Transaction ID: {payment.TransactionID}\n" +
                            $"Payment Method: {payment.PaymentMethod}\n" +
                            $"Amount Paid: ₹{payment.Amount}\n" +
                            $"Status: {payment.PaymentStatus}\n" +
                            $"Booking ID: {booking.BookingID}\n" +
                            $"Movie: {movie.Title}\n" +
                            $"Theatre: {theatre.TheatreName}\n" +
                            $"Seats: {booking.SeatNumbers} ({booking.SeatTypes})\n" +
                            $"Showtime: {showtime.StartTime}\n" +
                            $"Details: {payment.PaymentDetails}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePayment(int paymentId)
        {
            var payment = db.Payments.FirstOrDefault(p => p.PaymentID == paymentId);

            if (payment != null)
            {
                // Security check: ensure current user owns this payment
                var booking = db.Bookings.FirstOrDefault(b => b.BookingID == payment.BookingID);
                if (booking != null && booking.UserID == Convert.ToInt32(Session["UserID"]))
                {
                    // Delete payment
                    db.Payments.Remove(payment);

                    // Optionally delete booking
                    db.Bookings.Remove(booking);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("User_payment_detail");
        }

        public ActionResult CleanupOldPayments()
        {
            DateTime cutoffTime = DateTime.Now.AddHours(-24);

            var oldBookings = db.Bookings
                .Where(b => b.BookingDate < cutoffTime)
                .ToList();

            foreach (var booking in oldBookings)
            {
                // Remove related payments
                var payments = db.Payments.Where(p => p.BookingID == booking.BookingID).ToList();
                foreach (var payment in payments)
                {
                    db.Payments.Remove(payment);
                }

                // Remove the booking
                db.Bookings.Remove(booking);
            }

            db.SaveChanges();

            return Content("Old bookings and payments deleted successfully.");
        }


        public ActionResult About_us()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }



       
    }
}