using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DashboardApplication.DTOs;
using DashboardApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardApplication.Controllers
{
    /// <summary>
    /// The Controller class, it extends the ControllerBase Class
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Global variables
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly IUserService _userService;

        /// <summary>
        /// Contructor for UserController
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userService"></param>
        public UserController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        /// <summary>
        /// Users registration url
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // user/register
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterUser model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                {
                    throw new NullReferenceException("Invalid data.");
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                //var existingUser = _userManager.FindByEmailAsync(model.Email);

                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        Photo = model.Photo,
                        Gender = model.Gender
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    //var result = _userManager.CreateAsync(user, model.Password).Result;

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        return Ok("Registration Successful");
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }

            return BadRequest("Some properties are not valid.");
        }
        /// <summary>
        /// Update the users details route
        /// </summary>
        /// <param name="email"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// user/{email}
        [HttpPatch]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUser model)
        {
            if (ModelState.IsValid)
            {
                //var user = _userManager.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest("Invalid data.");
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);

                if (passwordCheck)
                {
                    user.FirstName = model.FirstName ?? user.FirstName;
                    user.LastName = model.LastName ?? user.LastName;
                    user.Photo = model.Photo ?? user.Photo;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return Ok("Update Successful");
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                else
                {
                    return BadRequest("Invalid password.");
                }
            }

            return BadRequest("Some properties are not valid");
        }
        /// <summary>
        /// Deletion of user's details route
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// user/{email}
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok("Successfully deleted.");
                }
                else
                {
                    return BadRequest("Deletion not successful.");
                }
            }           

            return BadRequest("User not found.");
        }

        /// <summary>
        /// Login route
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// user/login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUser model)
        {
            var _isAdmin = false;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user == null)
                {
                    return Unauthorized();
                }
                
                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!result)
                {
                    return Unauthorized();
                }

                var claims = new[]
                {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                // this encrypts the token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
                // Gets sign-in credentials for the server
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                // Creates Sucurity token descriptor
                var securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(10),
                    SigningCredentials = cred
                };
                // builds token handler
                var tokenHandler = new JwtSecurityTokenHandler();
                // creates token
                var token = tokenHandler.CreateToken(securityTokenDescriptor);

                var maleCount = 0;
                var femaleCount = 0;

                var allUsers = _userManager.Users;

                var users = new List<UserToReturn>();

                foreach (var r in allUsers)
                {
                    if (r.Gender == "Male")
                    {
                        maleCount++;
                    }
                    else
                    {
                        femaleCount++;
                    }
                    users.Add(
                        new UserToReturn
                        {
                            LastName = r.LastName,
                            FirstName = r.FirstName,
                            Email = r.Email,
                            Photo = r.Photo
                        });
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return Ok(new OperationResponse
                    {
                        Message = tokenHandler.WriteToken(token),
                        IsAdmin = true,
                        femaleCount = femaleCount,
                        maleCount = maleCount,
                        AllUsers = users,
                        Photo = user.Photo
                    }); ;
                }

                return Ok( new OperationResponse
                {
                    Message = tokenHandler.WriteToken(token),
                    IsAdmin = _isAdmin,
                    AllUsers = new List<UserToReturn>() { new UserToReturn { 
                        LastName = user.LastName,
                        FirstName = user.FirstName,
                        Email = user.Email,
                        Photo = user.Photo
                    } }
                });
            }

            return BadRequest("Incomplete details.");
        }
        
        
    }
}
