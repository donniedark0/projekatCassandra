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
    public class CommentController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public CommentController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("api/PostComment")]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            
            await _mapper.InsertAsync<Comment>(comment);
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", comment.HotelID);
            hotel.CommentIDs.Add(comment.CommentID);
            await _mapper.UpdateAsync<Hotel>("SET commentids = ? WHERE hotelid = ?", hotel.CommentIDs, hotel.HotelID);
 
            var user = await _mapper.FirstOrDefaultAsync<User>("WHERE userid = ?", comment.UserID);
            user.CommentIDs.Add(comment.CommentID);
            await _mapper.UpdateAsync<User>("SET commentids = ? WHERE userid = ?", user.CommentIDs, user.UserID);
            return StatusCode(204);
        }


        [Route("api/GetComments")]
        [HttpGet]
        public async Task<List<Comment>> GetComments()
        {
            var commentList = await _mapper.FetchAsync<Comment>();
            return commentList.ToList();
        }

        [Route("api/GetHotelsComments/{hotelid}")]
        [HttpGet]
        public async Task<List<Comment>> GetHotelsComments(string hotelid)
        {
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);
            List<Comment> commentList = new List<Comment>();
            foreach (var commentID in hotel.CommentIDs)
            {
                commentList.Add(await _mapper.FirstOrDefaultAsync<Comment>("WHERE commentid = ?", commentID));
            }

            return commentList;
        }

        [Route("DeleteComment/{commentid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(string commentid)
        {
            await _mapper.DeleteAsync<Comment>("WHERE commentid = ?", commentid);
            return StatusCode(204);
        }

        [Route("DeleteUsersComments/{username}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUsersComments(string username)
        {   
            var user = await _mapper.FirstOrDefaultAsync<User>("WHERE userid = ?", username);
            foreach (var commentID in user.CommentIDs)
            {
                await _mapper.DeleteAsync<Comment>("WHERE commentid = ?", commentID);
            }
            await _mapper.UpdateAsync<User>("SET commentids = ? WHERE userid = ?", null, user.UserID);
            return StatusCode(204);
        }

        [Route("DeleteHotelsComments/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotelsComments(string hotelid)
        {
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);
            foreach (var commentID in hotel.CommentIDs)
            {
                await _mapper.DeleteAsync<Comment>("WHERE commentid = ?", commentID);
            }
            await _mapper.UpdateAsync<Hotel>("SET commentids = ? WHERE hotelid = ?", null, hotel.HotelID);
 
            return StatusCode(204);
        }

        [Route("EditComment")]
        [HttpPut]
        public async Task<IActionResult> EditComment([FromBody] Comment comment)
        {
            await _mapper.UpdateAsync<Comment>("SET content = ? WHERE userid = ? AND commentid = ?", 
                                                comment.Content, comment.UserID, comment.CommentID);
            return StatusCode(204);
        }
    }
}
