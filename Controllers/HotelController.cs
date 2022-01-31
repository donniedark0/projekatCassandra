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

        [Route("api/PostHotel")]
        [HttpPost]
        public async Task<IActionResult> AddHotel([FromBody] Hotel hotel)
        {
            
            await _mapper.InsertIfNotExistsAsync<Hotel>(hotel);
            return StatusCode(204);
        }


        [Route("api/GetHotels")]
        [HttpGet]
        public async Task<List<Hotel>> GetHotels()
        {
            var hotelList = await _mapper.FetchAsync<Hotel>();
            return hotelList.ToList();
        }

/*
        [Route("DeleteHotel/{key}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(string key)
        {
        }

        [Route("EditHotel/{key}")]
        [HttpPut]
        public async Task<IActionResult> EditHotel(string key)
        {
        }*/
    }
}
