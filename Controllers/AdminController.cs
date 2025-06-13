using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Onlinemovietickitproject.Models;
using System.Data.Entity;

namespace Onlinemovietickitproject.Controllers
{
    public class AdminController : Controller
    {
        private readonly dbmovietickitEntities1 db = new dbmovietickitEntities1();



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

        // GET: Admin
        public ActionResult Index()
        {
            var totalUsers = db.Users.Count();
            var totalmovies = db.Movies.Count();
            var totaltheaters = db.Theatres.Count();

            // Get bookings for the last 6 months
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var bookings = db.Bookings
                .Where(b => b.BookingDate >= sixMonthsAgo)
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalTickets = g.Sum(b => b.SeatsBooked)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            ViewBag.Totaltheaters = totaltheaters;
            ViewBag.Totalmovies = totalmovies;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.BookingData = bookings;

            return View();
        }

        // GET: User_detail
        public ActionResult User_detail()
        {
            var users = db.Users.ToList();
            return View(users); // Pass user list to the view
        }



        public ActionResult Movie_detail()
        {
            var movies = db.Movies.ToList();
            return View(movies); // Pass user list to the view


        }

        [HttpPost]


        public ActionResult Movie_detail(Movy Movie_detail)
        {
            if (ModelState.IsValid)
            {
                if (Movie_detail.MovieID == 0)
                {
                    // INSERT
                    db.Movies.Add(Movie_detail);
                }
                else
                {
                    // UPDATE
                    var existingMovie = db.Movies.Find(Movie_detail.MovieID);
                    if (existingMovie != null)
                    {
                        existingMovie.Title = Movie_detail.Title;
                        existingMovie.Genre = Movie_detail.Genre;
                        existingMovie.Director = Movie_detail.Director;
                        existingMovie.Cast = Movie_detail.Cast;
                        existingMovie.Duration = Movie_detail.Duration;
                        existingMovie.ReleaseDate = Movie_detail.ReleaseDate;
                        existingMovie.Rating = Movie_detail.Rating;
                        existingMovie.ImageUrl = Movie_detail.ImageUrl;
                        existingMovie.Price = Movie_detail.Price;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Movie_detail");
            }

            return View("Movie_detail", db.Movies.ToList());
        }

        public ActionResult DeleteMovie(int id)
        {
            var movie = db.Movies.Find(id);
            if (movie != null)
            {
                db.Movies.Remove(movie);
                db.SaveChanges();
            }

            return RedirectToAction("Movie_detail");
        }


        public ActionResult Theater_detail()
        {
            var model = new TheaterManagementViewModel
            {
                Theatres = db.Theatres.Include(t => t.Showtimes).ToList(),
                Movies = db.Movies.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddTheatre(Theatre model)
        {
            if (ModelState.IsValid)
            {
                db.Theatres.Add(model);
                db.SaveChanges();
                return RedirectToAction("Theater_detail");
            }
            return View("Theater_detail", new TheaterManagementViewModel
            {
                Theatres = db.Theatres.Include(t => t.Showtimes).ToList(),
                Movies = db.Movies.ToList()
            });
        }

        [HttpPost]
        public ActionResult UpdateTheatre(Theatre model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Theater_detail");
            }
            return View("Theater_detail", new TheaterManagementViewModel
            {
                Theatres = db.Theatres.Include(t => t.Showtimes).ToList(),
                Movies = db.Movies.ToList()
            });
        }

        public ActionResult DeleteTheatre(int id)
        {
            var theatre = db.Theatres.Find(id);
            if (theatre != null)
            {
                db.Theatres.Remove(theatre);
                db.SaveChanges();
            }
            return RedirectToAction("Theater_detail");
        }

        public ActionResult GetShowtimesByTheatre(int theatreId)
        {
            var showtimes = db.Showtimes
                .Include(s => s.Movy)
                .Where(s => s.TheatreID == theatreId)
                .ToList();

            return Json(showtimes.Select(s => new {
                s.ShowtimeID,
                Movy = s.Movy != null ? new { s.Movy.Title } : null,
                Date = s.Date.ToString("yyyy-MM-dd"),
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                s.MovieID
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddShowtime(Showtime model)
        {
            if (ModelState.IsValid)
            {
                db.Showtimes.Add(model);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpPost]
        public ActionResult EditShowtime(Showtime model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpPost]
        public ActionResult DeleteShowtime(int id)
        {
            var showtime = db.Showtimes.Find(id);
            if (showtime != null)
            {
                db.Showtimes.Remove(showtime);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }







    }
}