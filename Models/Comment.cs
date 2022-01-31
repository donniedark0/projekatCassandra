using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Comment
    {
        public string CommentID { get; set; }
        public string UserID { get; set; }
        public string HotelID { get; set; }
        public string Content { get; set; }
    }
}
