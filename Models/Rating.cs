using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Rating
    {
        public string RatingID { get; set; }
        public string HotelID { get; set; }
        public string UserID { get; set; }
        public string Mark { get; set; }
    }
}
