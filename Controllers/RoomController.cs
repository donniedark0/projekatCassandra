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


        [Route("api/GetHotelsRooms/{hotelid}")]
        [HttpGet]
        public async Task<List<Room>> GetHotelsRooms(string hotelid)
        {
            var roomList = await _mapper.FetchAsync<Room>("WHERE hotelid = ?", hotelid);
            return roomList.ToList();
        }

        [Route("api/GetAvailableHotelsRooms/{hotelid}")]
        [HttpGet]
        public async Task<List<Room>> GetHotelsRooms(string hotelid, [FromBody] string numOfBeds, DateTime dateFrom, DateTime dateTo)
        {
            var roomList = await _mapper.FetchAsync<Room>("WHERE hotelid = ?", hotelid);
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);

            List<Reservation> allReservations = new List<Reservation>();

            foreach (string resID in hotel.ReservationIDs)
            {
                allReservations.Add(await _mapper.FirstOrDefaultAsync<Reservation>("WHERE reservationid = ?", resID));
            }
            List<Room> filteredRoomList = new List<Room>();
            foreach (Room room in roomList)
            {
                if (room.NumOfBeds == numOfBeds){
                    foreach (Reservation reservation in allReservations)
                    {
                        if(dateTo < reservation.DateFrom || dateFrom > reservation.DateTo){
                            filteredRoomList.Append(room);
                        }
                    }
                }
            }
            return filteredRoomList;
        }


        [Route("DeleteRoom/{roomid}/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(string roomid, string hotelid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ? AND roomid = ?", hotelid, roomid);
            return StatusCode(204);
        }
        [Route("DeleteHotelsRooms/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotelsRooms(string hotelid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ?", hotelid);
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
