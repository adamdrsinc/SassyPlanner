using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class Discount
    {

        [Key]
        public int DiscountID { get; set; }

        [Required, MaxLength(100)]
        public string DiscountName { get; set; } = string.Empty;

        [Required]
        public decimal DiscountPercentage { get; set; }



    }
}
