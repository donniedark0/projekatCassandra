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

        [Route("PostRoom")]
        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            
            await _mapper.InsertAsync<Room>(room);
            return StatusCode(204);
        }


        [Route("GetHotelsRooms/{hotelid}")]
        [HttpGet]
        public async Task<List<Room>> GetHotelsRooms(string hotelid)
        {
            var roomList = await _mapper.FetchAsync<Room>("WHERE hotelid = ?", hotelid);
            return roomList.ToList();
        }

        [Route("GetAvailableHotelsRooms/{hotelid}")]
        [HttpPut]
        public async Task<List<Room>> GetHotelsRooms(string hotelid, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo, [FromQuery] string numOfBeds)
        {
            var roomList = (await _mapper.FetchAsync<Room>("WHERE hotelid = ?", hotelid)).ToList();
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);
            List<Reservation> allReservations = new List<Reservation>();

            foreach (string resID in hotel.Reservations)
            {
                allReservations.Add(await _mapper.FirstOrDefaultAsync<Reservation>("WHERE reservationid = ?", resID));
            }
            List<Room> filteredRoomList = new List<Room>();
            
            foreach (Room room in roomList.Where(r => r.NumOfBeds == numOfBeds))
            {
                int i = 0;
                foreach (Reservation reservation in allReservations.Where(res => res.RoomID == room.RoomID))
                {
                    if((dateTo >= reservation.DateFrom && dateFrom <= reservation.DateFrom) || (dateFrom <= reservation.DateTo && dateTo >= reservation.DateTo))
                        i = 1;
                }
                if(i == 0)
                {
                    filteredRoomList.Add(room);
                }
            }
            return filteredRoomList;
        }

        [Route("GetRoomNumber")]
        [HttpGet]
        public async Task<int> GetRoomNumber()
        {
            var roomList = await _mapper.FetchAsync<Room>();
            return roomList.Count();
        }


        [Route("DeleteRoom/{roomid}/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(string roomid, string hotelid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ? AND roomid = ?", hotelid, roomid);

            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);

            var allReservations = await _mapper.FetchAsync<Reservation>();

            foreach (Reservation reservation in allReservations.Where(reservation => reservation.RoomID == roomid))
            {   
                await _mapper.DeleteAsync<Reservation>("WHERE reservationid = ?", reservation.ReservationID);
            }

            return StatusCode(204);
        }
        [Route("DeleteHotelsRooms/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotelsRooms(string hotelid)
        {
            await _mapper.DeleteAsync<Room>("WHERE hotelid = ?", hotelid);

            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);

            foreach (string resID in hotel.Reservations)
            {
                await _mapper.DeleteAsync<Reservation>("WHERE reservationid = ?", resID);
            }

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
