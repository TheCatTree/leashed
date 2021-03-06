using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace leashApi.Models
{
    public class Picture
    {
        public int Id {get; set;}

        [Required]
        [StringLength(255)]
        public string GivenName { get; set;}

        [Required]
        [StringLength(255)]
        public string FileName { get; set;}

        [Required]
        [StringLength(255)]
        public string URL { get; set;}

        //public int UserDataId { get; set;}

        public IList<PictureDogJoin> PictureDogJoins {get; set;}

        public Picture(){
            PictureDogJoins = new Collection<PictureDogJoin>();
        }
    }
}