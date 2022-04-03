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
    public class SeasonController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public SeasonController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("PostSeason")]
        [HttpPost]
        public async Task<IActionResult> AddSeason([FromBody] Season season)
        {
            
            await _mapper.InsertAsync<Season>(season);
            return StatusCode(204);
        }


        [Route("GetSeasons")]
        [HttpGet]
        public async Task<List<Season>> GetSeasons()
        {
            var seasonList = await _mapper.FetchAsync<Season>();
            return seasonList.ToList();
        }

        [Route("GetSeasonNumber")]
        [HttpGet]
        public async Task<int> GetSeasonNumber()
        {
            var seasonList = await _mapper.FetchAsync<Season>();
            return seasonList.Count();
        }

        [Route("DeleteSeason/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSeason(string ID)
        {
            await _mapper.DeleteAsync<Season>("WHERE seasonid = ?", ID);
            return StatusCode(204);
        }

        [Route("EditSeason")]
        [HttpPut]
        public async Task<IActionResult> EditSeason([FromBody] Season season)
        {
            await _mapper.UpdateAsync<Season>("SET name = ? WHERE seasonid = ?", 
                                                season.Name, season.SeasonID);
            return StatusCode(204);
        }
    }
}
