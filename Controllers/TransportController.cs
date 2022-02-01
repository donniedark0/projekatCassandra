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
    public class TransportController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public TransportController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("api/PostTransport")]
        [HttpPost]
        public async Task<IActionResult> AddTransport([FromBody] Transport transport)
        {
            
            await _mapper.InsertAsync<Transport>(transport);
            return StatusCode(204);
        }


        [Route("api/GetTransports")]
        [HttpGet]
        public async Task<List<Transport>> GetTransports()
        {
            var transportList = await _mapper.FetchAsync<Transport>();
            return transportList.ToList();
        }


        [Route("DeleteTransport/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTransport(string ID)
        {
            await _mapper.DeleteAsync<Transport>("WHERE transportid = ?", ID);
            return StatusCode(204);
        }

        [Route("EditTransport")]
        [HttpPut]
        public async Task<IActionResult> EditTransport([FromBody] Transport transport)
        {
            await _mapper.UpdateAsync<Transport>("SET type = ? WHERE transportid = ?", 
                                                transport.Type, transport.TransportID);
            return StatusCode(204);
        }
    }
}
