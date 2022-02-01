using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Reservation
    {
        public string ReservationID { get; set; }
        public string HotelID { get; set; }
        public string RoomID { get; set; }
        public string UserID { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }
    }
}
