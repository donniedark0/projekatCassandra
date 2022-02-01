using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Hotel
    {
        public string HotelID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public List<string> CommentIDs { get; set; }
        public List<string> RatingIDs { get; set; }
        public string LocationID { get; set; }
        public string TransportID { get; set; }
    }
}
