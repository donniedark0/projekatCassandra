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
    public class RoomController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public RoomController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("api/PostRoom")]
        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            
            await _mapper.InsertAsync<Room>(room);
            return StatusCode(204);
        }


        [Route("api/GetHotelsRooms/{hid}")]
        [HttpGet]
        public async Task<List<Room>> GetHotelsRooms(string hid)
        {
            var roomList = await _mapper.FetchAsync<Room>("WHERE hotelid = ?", hid);
            return roomList.ToList();
        }


        [Route("DeleteRoom/{rid}/{hid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(string rid, string hid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ? AND roomid = ?", hid, rid);
            return StatusCode(204);
        }
        [Route("DeleteHotelsRooms/{hid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotelsRooms(string hid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ?", hid);
            return StatusCode(204);
        }

        [Route("EditRoom")]
        [HttpPut]
        public async Task<IActionResult> EditRoom([FromBody] Room Room)
        {
            await _mapper.UpdateAsync<Room>("SET number = ?, numofbeds = ? WHERE hotelid = ? AND roomid = ?", 
                                                Room.Number, Room.NumOfBeds, Room.HotelID, Room.RoomID);
            return StatusCode(204);
        }
    }
}
