using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Onlinemovietickitproject.Models
{
    public class PaymentViewModel
    {
        [Required]
        public List<string> SelectedSeats { get; set; } = new List<string>();

        [Required]
        public List<bool> IsVip { get; set; } = new List<bool>();

        [Required]
        public string MovieTitle { get; set; }

        [Required]
        public string TheaterName { get; set; }

        [Required]
        public int ShowtimeID { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal GrandTotal { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; }

        // Card payment fields
        [RequiredIf("PaymentMethod == 'Card'", ErrorMessage = "Card number is required")]
        [CreditCard(ErrorMessage = "Invalid card number")]
        public string CardNumber { get; set; }

        [RequiredIf("PaymentMethod == 'Card'", ErrorMessage = "Cardholder name is required")]
        public string CardName { get; set; }

        [RequiredIf("PaymentMethod == 'Card'", ErrorMessage = "Expiry date is required")]
        public string ExpiryDate { get; set; }

        [RequiredIf("PaymentMethod == 'Card'", ErrorMessage = "CVV is required")]
        public string CVV { get; set; }

        // UPI fields
        [RequiredIf("PaymentMethod == 'UPI'", ErrorMessage = "UPI ID is required")]
        public string UpiId { get; set; }

        // Net banking fields
        [RequiredIf("PaymentMethod == 'Netbanking'", ErrorMessage = "Bank code is required")]
        public string BankCode { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

       
        public decimal ConvenienceFee { get; set; }
        public decimal Tax { get; set; }
      

    }
}