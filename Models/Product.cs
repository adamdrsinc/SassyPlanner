using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class Product
    {

        [Key]
        public int ProductID { get; set; }

        [Required, MaxLength(255)]
        public string ProductName { get; set; } = "";

        [Required, MaxLength(255)]
        public string ProductImageDescription { get; set; } = string.Empty;

        [Required]
        public byte[] ProductImageData { get; set; } = new byte[0];

        [Required]
        public decimal ProductPrice { get; set; } = 0.00m;

        [Required, MaxLength(100)]
        public string ProductType { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Style { get; set; } = string.Empty;
 

    }
}
