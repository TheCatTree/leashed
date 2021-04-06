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
        public TokenSub TokenSub { get; set; }
        public ICollection<Picture> Pictures {get; set;}
        public ICollection<Dog> Dogs {get; set;}
        public ICollection<UserData> friends {get; set;}
        public ParkItem Park {get; set;}

        public UserData()
        {
            Pictures = new Collection<Picture>();
            Dogs = new Collection<Dog>();
            friends = new Collection<UserData>();
        }

    }
}