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
    public class LocationController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public LocationController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("api/PostLocation")]
        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            
            await _mapper.InsertAsync<Location>(location);
            return StatusCode(204);
        }


        [Route("api/GetLocations")]
        [HttpGet]
        public async Task<List<Location>> GetLocations()
        {
            var locationList = await _mapper.FetchAsync<Location>();
            return locationList.ToList();
        }

        [Route("api/GetLocationsBySeason/{season}")]
        [HttpGet]
        public async Task<List<Location>> GetLocationsBySeason(string season)
        {
            var locationList = await _mapper.FetchAsync<Location>("WHERE season = ?", season);
            return locationList.ToList();
        }


        [Route("DeleteLocation/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLocation(string ID)
        {
            await _mapper.DeleteAsync<Location>("WHERE locationid = ?", ID);
            return StatusCode(204);
        }

        [Route("EditLocation")]
        [HttpPut]
        public async Task<IActionResult> EditLocation([FromBody] Location location)
        {
            await _mapper.UpdateAsync<Location>("SET season = ?, city = ?, state = ? WHERE locationid = ?", 
                                                location.Season, location.City, location.State, location.LocationID);
            return StatusCode(204);
        }
    }
}
