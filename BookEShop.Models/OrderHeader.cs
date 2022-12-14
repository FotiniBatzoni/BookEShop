using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        // [Required]
        // public string ApplicationUserId {get; set;}

        public string? user { get; set; }
        // [ForeignKey("ApplicationUserId")]
        //[ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }    


        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime ShippingDate { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }    

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }    


    }


}
