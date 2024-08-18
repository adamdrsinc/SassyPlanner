using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class BasketItem
    {

        [Key, Required]
        public int BasketItemID { get; set; }

        [Required]
        public int BasketID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, MaxLength(100)]
        public string Personalisation { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string SpiralColour { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string StartMonth { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string InternalPages { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string PlannerSize { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string AdditionalInfo { get; set; } = string.Empty;

        [Required]
        public decimal ItemPrice { get; set; }

    }
}
