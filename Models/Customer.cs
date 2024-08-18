using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class Customer
    {

        [Key]
        public int CustomerID { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = "";

        [Required, MaxLength(150)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(150)]
        public string LastName { get; set; } = "";

        [MaxLength(255)]
        public string AdditionalInfo { get; set; } = "";

        [Required, MaxLength(200)]
        public string AddressLineOne { get; set; } = "";

        [MaxLength(200)]
        public string AddressLineTwo { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string City { get; set; } = "";

        [Required, StringLength(10, MinimumLength = 4)]
        public string Postcode { get; set; } = "";

        [Required]
        public int BasketID { get; set; }


    }
}
