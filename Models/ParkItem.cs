using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace leashApi.Models
{
    public class ParkItem
    {
        public long Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string IsLeashed { get; set; }
        public bool RoadFront { get; set; }
        public string Suburb { get; set; }
        [Required]
        [StringLength(255)]
        public string City { get; set; }
        [Required]
        [StringLength(255)]
        public string Country { get; set; }

        public IList<UserData> ParkGoers {get; set; }

        public ParkItem(){
            ParkGoers = new List<UserData>();
        }
    }
}