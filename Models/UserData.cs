using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace leashApi.Models
{
    public class UserData
    {
        
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string TokenSub { get; set; }
        public ICollection<Picture> Pictures {get; set;}

        public UserData()
        {
            Pictures = new Collection<Picture>();
        }

    }
}