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
    public class UserController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public UserController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("api/PostUser")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            
            await _mapper.InsertAsync<User>(user);
            return StatusCode(204);
        }


        [Route("api/GetUsers")]
        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            var userList = await _mapper.FetchAsync<User>();
            return userList.ToList();
        }


        [Route("DeleteUser/{ID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string ID)
        {
            await _mapper.DeleteAsync<User>("WHERE userid = ?", ID);
            return StatusCode(204);
        }

        [Route("EditUser")]
        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] User user)
        {
            await _mapper.UpdateAsync<User>("SET name = ?, surname = ?, phone = ?, password = ? WHERE userid = ?", 
                                                user.Name, user.Surname, user.Phone, user.Password, user.UserID);
            return StatusCode(204);
        }
    }
}
