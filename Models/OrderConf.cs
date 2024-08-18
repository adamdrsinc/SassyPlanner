using System.ComponentModel.DataAnnotations;

//Created by Adam Sinclair

namespace currentworkingsassyplanner.Models
{
    public class OrderConf
    {
        [Key, Required]
        public int OrderID {  get; set; }
        [Required]
        public decimal OrderTotal { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required, StringLength(660)]
        public string DeliveryAddress { get; set; }
    }
}
