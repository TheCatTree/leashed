using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace leashApi.Models
{
    public class Dog
    {
        
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        public int UserDataId { get; set;}
        public IList<PictureDogJoin> PictureDogJoins {get; set;}

        public Dog()
        {
            PictureDogJoins = new Collection<PictureDogJoin>();
        }
    }
}