using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projekat_cassandra.Models;
using Cassandra;
using Cassandra.Mapping;

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
            location.HotelIDs.Add(hotel.HotelID);
            await _mapper.UpdateAsync<Hotel>("SET hotelids = ? WHERE locationid = ?", location.HotelIDs, locationid);
            return StatusCode(204);
        }


        [Route("GetHotelsByLocation")]
        [HttpGet]
        public async Task<List<Hotel>> GetHotels([FromBody] List<string> hotelIDs)
        {
            List<Hotel> hotelList = new List<Hotel>();
            foreach (string hotelid in hotelIDs)
            {
                hotelList.Add(await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid));
            }
            return hotelList;
        }


        [Route("DeleteHotel/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(string ID)
        {
            await _mapper.DeleteAsync<Hotel>("WHERE hotelid = ?", ID);
            return StatusCode(204);
        }

        [Route("EditHotel")]
        [HttpPut]
        public async Task<IActionResult> EditHotel([FromBody] Hotel hotel)
        {
            await _mapper.UpdateAsync<Hotel>("SET name = ?, picture = ?, phone = ?, commentids = ?, ratingids = ?, transportIDs = ? WHERE hotelid = ?", 
                                                hotel.Name, hotel.Picture, hotel.Phone, hotel.CommentIDs, hotel.RatingIDs, hotel.TransportIDs, hotel.HotelID);
            return StatusCode(204);
        }
    }
}
