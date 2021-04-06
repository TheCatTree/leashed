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
        public string Key { get; set;}

        public bool IsPublic { get; set; } = true;

        [Required]
        [StringLength(255)]
        public string FileName { get; set;}

        public int UserDataId { get; set;}

        public IList<PictureDogJoin> PictureDogJoins {get; set;}
        public IList<TokenSub> canRead {get; set;}
        public IList<TokenSub>  canEdit {get; set;}
        

        public Picture(){
            PictureDogJoins = new Collection<PictureDogJoin>();
            canRead = new List<TokenSub>();
            canEdit = new List<TokenSub>();

            
        }
    }
}