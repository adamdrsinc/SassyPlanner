using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class Basket
    {

        [Key]
        public int BasketID { get; set; }

        [Required, MaxLength(32)]
        public string CookieID { get; set; } = string.Empty;



    }
}
