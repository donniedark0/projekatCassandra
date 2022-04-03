using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projekat_cassandra.Models;
using Cassandra;
using Cassandra.Mapping;
using System.Diagnostics;

namespace projekat_cassandra.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public HotelController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("PostHotel/{locationid}")]
        [HttpPost]
        public async Task<IActionResult> AddHotel(string locationid, [FromBody] Hotel hotel)
        {
            await _mapper.InsertAsync<Hotel>(hotel);
            var location = await _mapper.FirstOrDefaultAsync<Location>("WHERE locationid = ?", locationid);
            if(location.HotelIDs == null){
                location.HotelIDs = new List<string>();
            }
            location.HotelIDs.Add(hotel.HotelID);
            await _mapper.UpdateAsync<Location>("SET hotelIDs = ? WHERE locationid = ?", location.HotelIDs, locationid);
            return StatusCode(204);
        }

        [Route("GetHotelNumber")]
        [HttpGet]
        public async Task<int> GetHotelNumber()
        {
            var hotelList = await _mapper.FetchAsync<Hotel>();
            return hotelList.Count();
        }

        [Route("GetHotelsByLocations")]
        [HttpPut]
        public async Task<List<Hotel>> GetHotelIDs([FromBody] List<string> locationIDs)
        {
            List<Hotel> hotelList = new List<Hotel>();
            List<Location> locationList = new List<Location>();
            foreach (string lid in locationIDs)
            {
                var location = await _mapper.FirstOrDefaultAsync<Location>("WHERE locationid = ?", lid);
                foreach (var hid in location.HotelIDs)
                {
                    hotelList.Add(await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hid));
                }
            }
            return hotelList;
        }


        [Route("DeleteHotel/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(string ID)
        {
            await _mapper.DeleteAsync<Hotel>("WHERE hotelid = ?", ID);

            var allLocations = (await _mapper.FetchAsync<Location>()).ToList();
            foreach(Location location in allLocations.Where(loc => loc.HotelIDs.Exists(e => e == ID))){
                location.HotelIDs.Remove(ID);
                await _mapper.UpdateAsync<Location>("SET hotelids = ? WHERE locationid = ?", location.HotelIDs, location.LocationID);
            }
            return StatusCode(204);
        }

        [Route("EditHotel")]
        [HttpPut]
        public async Task<IActionResult> EditHotel([FromBody] Hotel hotel)
        {
            await _mapper.UpdateAsync<Hotel>("SET name = ?, picture = ? WHERE hotelid = ?", 
                                                hotel.Name, hotel.Picture, hotel.HotelID);
            return StatusCode(204);
        }
    }
}
