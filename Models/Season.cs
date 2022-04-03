using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat_cassandra.Models
{
    public class Season
    {
        public string SeasonID { get; set; }
        public string Name { get; set; }
        public List<string> LocationIDs { get; set; }
    }
}
