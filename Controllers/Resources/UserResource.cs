using System.Collections.Generic;
using System.Collections.ObjectModel;
using leashApi.Models;

namespace leashed.Controllers.Resources
{
        public class UserResource
    {
 
        public int Id { get; set; }
       
        public string Name { get; set; }
       
        public TokenSub TokenSub { get; set; }
        public ICollection<PictureResource> Pictures {get; set;}
        public ICollection<DogResource> Dogs {get; set;}
        public ICollection<int> friends {get; set;}
        public int Park {get; set;}

        public UserResource()
        {
            Pictures = new Collection<PictureResource>();
            Dogs = new Collection<DogResource>();
            friends = new Collection<int>();
        }

    }

}