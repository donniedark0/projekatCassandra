using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public List<string> RatingIDs { get; set; }
        public List<string> CommentIDs { get; set; }
        public List<string> Reservations { get; set; }
    }
}
