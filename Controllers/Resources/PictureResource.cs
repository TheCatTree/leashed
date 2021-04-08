using System.Collections.Generic;
using System.Collections.ObjectModel;
using leashApi.Models;

namespace leashed.Controllers.Resources
{
    public class PictureResource
    {
         public int Id {get; set;}

        public string Key { get; set;}

        public bool IsPublic { get; set; } = true;

        public string FileName { get; set;}

        public int UserDataId { get; set;}

        public IList<int> DogsInPicture {get; set;}
        public IList<TokenSub> canRead {get; set;}
        public IList<TokenSub>  canEdit {get; set;}
        

        public PictureResource(){
            DogsInPicture = new Collection<int>();
            canRead = new List<TokenSub>();
            canEdit = new List<TokenSub>();

            
        }
    }
}