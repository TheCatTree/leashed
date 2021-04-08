using System.Collections.Generic;

namespace leashed.Controllers.Resources
{
    public class ParkItemResource
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IsLeashed { get; set; }
        public bool RoadFront { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public IList<UserResource> ParkGoers {get; set; }

        public ParkItemResource(){
            ParkGoers = new List<UserResource>();
        }
    }
}