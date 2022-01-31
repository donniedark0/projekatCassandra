using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Room
    {
        public string RoomID { get; set; }
        public string Number { get; set; }
        public string NumOfBeds { get; set; }
        public string HotelID { get; set; }
    }
}
