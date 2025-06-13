using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Onlinemovietickitproject.Models
{
    public class BookingConfirmationViewModel
    {
        public int BookingID { get; set; }
        public string MovieTitle { get; set; }
        public string TheaterName { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<string> SeatsBooked { get; set; }
        public List<string> SeatTypes { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionID { get; set; }
        public DateTime BookingDate { get; set; }



        public Booking Booking { get; set; }
        public Payment Payment { get; set; }
        public string TheatreName { get; set; }
    }

}