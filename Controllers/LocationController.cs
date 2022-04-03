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

        [Route("PostLocation/{seasonid}")]
        [HttpPost]
        public async Task<IActionResult> AddLocation(string seasonid, [FromBody] Location location)
        {
            
            await _mapper.InsertAsync<Location>(location);
            var season = await _mapper.FirstOrDefaultAsync<Season>("WHERE seasonid = ?", seasonid);
            season.LocationIDs.Add(location.LocationID);
            await _mapper.UpdateAsync<Season>("SET locationids = ? WHERE seasonid = ?", season.LocationIDs, seasonid);
            return StatusCode(204);
        }


        [Route("GetLocations")]
        [HttpGet]
        public async Task<List<Location>> GetLocations()
        {
            var locationList = await _mapper.FetchAsync<Location>();
            return locationList.ToList();
        }

        [Route("GetLocationNumber")]
        [HttpGet]
        public async Task<int> GetLocationNumber()
        {
            var locationList = await _mapper.FetchAsync<Location>();
            return locationList.Count();
        }

        [Route("GetLocationsByIDs")]
        [HttpPut]
        public async Task<List<Location>> GetLocationsByIDs([FromBody] List<string> ids)
        {
            List<Location> locationList = new List<Location>();
            foreach (string locationid in ids)
            {
                locationList.Add(await _mapper.FirstOrDefaultAsync<Location>("WHERE locationid = ?", locationid));
            }
            return locationList;
        }

        /*[Route("api/GetLocationsBySeason/{season}")]
        [HttpGet]
        public async Task<List<Location>> GetLocationsBySeason(string season)
        {
            var locationList = await _mapper.FetchAsync<Location>("WHERE season = ?", season);
            return locationList.ToList();
        }*/


        [Route("DeleteLocation/{seasonID}/{locationID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLocation(string seasonID, string locationID)
        {
            await _mapper.DeleteAsync<Location>("WHERE locationid = ?", locationID);

            var season = await _mapper.FirstOrDefaultAsync<Season>("WHERE seasonid = ?", seasonID);

            season.LocationIDs.Remove(locationID);

            await _mapper.UpdateAsync<Season>("SET locationids = ? WHERE seasonid = ?", season.LocationIDs, seasonID);

            return StatusCode(204);
        }

        [Route("EditLocation")]
        [HttpPut]
        public async Task<IActionResult> EditLocation([FromBody] Location location)
        {
            await _mapper.UpdateAsync<Location>("SET city = ?, state = ? WHERE locationid = ?", 
                                                location.City, location.State, location.LocationID);
            return StatusCode(204);
        }
    }
}
