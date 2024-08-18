using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class CustomerOrder
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public decimal OrderPrice { get; set; }

    }
}
