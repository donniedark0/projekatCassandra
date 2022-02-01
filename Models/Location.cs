using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Location
    {
        public string LocationID { get; set; }
        public string Season { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public List<string> HotelIDs { get; set; }
    }
}
