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

        [Route("PostUser")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            
            await _mapper.InsertAsync<User>(user);
            return StatusCode(204);
        }


        [Route("GetUser/{username}/{password}")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUsers(string username, string password)
        {
            var userList = await _mapper.FetchAsync<User>();
            var userForSending = new User();

            foreach (User user in userList.Where(u => u.Username == username && u.Password == password))
            {
                userForSending = user;   
            }
            if(userForSending.UserID == null)
                return StatusCode(404);
            else
                return userForSending;
        }

        [Route("GetUserNumber")]
        [HttpGet]
        public async Task<int> GetUserNumber()
        {
            var userList = await _mapper.FetchAsync<User>();
            return userList.Count();
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
            await _mapper.UpdateAsync<User>("SET username = ?, phone = ?, password = ?, ratingids = ? WHERE userid = ?", 
                                                user.Username, user.Phone, user.Password, user.RatingIDs, user.UserID);
            return StatusCode(204);
        }
    }
}
