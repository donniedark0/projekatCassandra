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
    public class ReservationController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public ReservationController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        /*[Route("api/PostReservation")]
        [HttpPost]
        public async Task<IActionResult> AddReservation([FromBody] Reservation reservation)
        {
            
            await _mapper.InsertAsync<Reservation>(reservation);
            
            return StatusCode(204);
        }


        [Route("api/GetUsersReservations/{username}")]
        [HttpGet]
        public async Task<List<Reservation>> GetUsersReservations(string username)
        {
            var reservationList = await _mapper.FetchAsync<Reservation>("WHERE userid = ?", username);
            return reservationList.ToList();
        }

        [Route("api/GetHotelsReservations/{hotelid}")]
        [HttpGet]
        public async Task<List<Reservation>> GetHotelsReservations(string hotelid)
        {
            var reservationList = await _mapper.FetchAsync<Reservation>("WHERE hotelid = ?", hotelid);
            return reservationList.ToList();
        }


        [Route("DeleteReservation/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteReservation(string ID)
        {
            await _mapper.DeleteAsync<Reservation>("WHERE reservationid = ?", ID);
            return StatusCode(204);
        }*/

        [Route("EditReservation")]
        [HttpPut]
        public async Task<IActionResult> EditReservation([FromBody] Reservation reservation)
        {
            await _mapper.UpdateAsync<Reservation>("SET dateffrom = ?, dateto = ? WHERE Reservationid = ?", 
                                                reservation.DateFrom, reservation.DateTo, reservation.ReservationID);
            return StatusCode(204);
        }
    }
}
