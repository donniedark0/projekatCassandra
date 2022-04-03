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
    public class RatingController : Controller
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        public RatingController()
        {
            _cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithPort(9042).Build();
            _session = _cluster.Connect("traveldotcom");
            _mapper = new Mapper(_session);
        }

        [Route("PostRating/{userid}/{hotelid}")]
        [HttpPost]
        public async Task<IActionResult> AddRating(string userid, string hotelid, [FromBody] Rating rating)
        {
            
            await _mapper.InsertAsync<Rating>(rating);
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);
            hotel.RatingIDs.Add(rating.RatingID);
            await _mapper.UpdateAsync<Hotel>("SET ratingids = ? WHERE hotelid = ?", hotel.RatingIDs, hotel.HotelID);

            var user = await _mapper.FirstOrDefaultAsync<User>("WHERE userid = ?", userid);
            user.RatingIDs.Add(rating.RatingID);
            await _mapper.UpdateAsync<User>("SET ratingids = ? WHERE userid = ?", user.RatingIDs, user.UserID);


            return StatusCode(204);
        }

        [Route("GetRatingNumber")]
        [HttpGet]
        public async Task<int> GetRatingNumber()
        {
            var ratingList = await _mapper.FetchAsync<Rating>();
            return ratingList.Count();
        }


        [Route("ClaculateHotelRatings/{hotelid}")]
        [HttpPut]
        public async Task<int> ClaculateRatings(string hotelid)
        {
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);

            List<Rating> ratingList = new List<Rating>();
            int finalRating = 0;
            foreach (string ratingID in hotel.RatingIDs)
            {
                ratingList.Add(await _mapper.FirstOrDefaultAsync<Rating>("WHERE ratingid = ?", ratingID));
            }

            foreach (Rating rating in ratingList)
            {
                finalRating += int.Parse(rating.Mark);
            }

            return finalRating/ratingList.Count();
        }

        [Route("DeleteRating/{ratingid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRating(string ratingid)
        {
            await _mapper.DeleteAsync<Rating>("WHERE ratingid = ?", ratingid);
            return StatusCode(204);
        }

        [Route("DeleteUsersRatings/{username}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUsersRatings(string username)
        {
            var user = await _mapper.FirstOrDefaultAsync<User>("WHERE userid = ?", username);
            foreach (var ratingID in user.RatingIDs)
            {
                await _mapper.DeleteAsync<Rating>("WHERE ratingid = ?", ratingID);
            }
            await _mapper.UpdateAsync<User>("SET ratingids = ? WHERE userid = ?", null, user.UserID);
            return StatusCode(204);
        }

        [Route("DeleteHotelsRatings/{hotelid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHotelsRatings(string hotelid)
        {
            var hotel = await _mapper.FirstOrDefaultAsync<Hotel>("WHERE hotelid = ?", hotelid);
            foreach (var ratingID in hotel.RatingIDs)
            {
                await _mapper.DeleteAsync<Rating>("WHERE ratingid = ?", ratingID);
            }
            return StatusCode(204);
        }

        [Route("EditRating")]
        [HttpPut]
        public async Task<IActionResult> EditRating([FromBody] Rating rating)
        {
            await _mapper.UpdateAsync<Rating>("SET mark = ? WHERE ratingid = ?", rating.Mark, rating.RatingID);
            return StatusCode(204);
        }
    }
}
