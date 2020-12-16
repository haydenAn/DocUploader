using UploaderApp.API.Models;
using UploaderApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using UploaderApp.API.Helpers;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using MongoDB.Bson;
using System.IO;

namespace UploaderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //api/user
        [HttpGet]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser([FromQuery] string email) {
            
            var  user = await _userService.GetUserByEmail(email);

           return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            string s1 = $"Add User email= {user.EmailAddress}, id = {user.Id}";
  
            var result = await _userService.CreateUser(user);

            if ( result!= null )
            {
                Debug.WriteLine(s1);
                return Ok(result);
                //  return Created(nameof(GetEmailLink), new {  emaillink = sLink , id = doc.Id.ToString() , document = doc});
            }

            return BadRequest("Error adding new user to database");
        }
 
    }
}