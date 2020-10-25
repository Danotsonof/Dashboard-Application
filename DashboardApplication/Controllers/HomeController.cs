using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DashboardApplication.Models;
using DashboardApplication.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using DashboardApplication.DTOs;
using System.Collections;
using Microsoft.AspNetCore.Http;

namespace DashboardApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string apiBaseUrl;
        public HomeController(ILogger<HomeController> logger,
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            apiBaseUrl = configuration.GetValue<string>("WebAPIBaseUrl");
        }
        /// <summary>
        /// Loads the profile View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("Profile")]
        private ActionResult Profile(UserToReturn model)
        {
            var user = new UserToReturn
            {
                FirstName = model.FirstName,
                Email = model.Email
            };
            //UserInfo user = JsonConvert.DeserializeObject<UserInfo>(Convert.ToString(TempData["Profile"]));
            return View(user);
        }
        /// <summary>
        /// Loads the Dashboard view
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public ActionResult Dashboard(OperationResponse op)
        {
            return View(op);
        }
        /// <summary>
        /// Loads the login view
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// Posts route for of the login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            HttpClient client = new HttpClient();
            //using (HttpClient client = new HttpClient())
            //{
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "/user/login";
            
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //TempData["Profile"] = JsonConvert.SerializeObject(Response.Content);
                        var customerJsonString = await Response.Content.ReadAsStringAsync();
                        //Console.WriteLine("Your response data is: " + customerJsonString);

                        // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                        var deserialized = JsonConvert.DeserializeObject<OperationResponse>(custome‌​rJsonString);
                    HttpContext.Session.SetString("token", deserialized.Message);
                        if (deserialized.IsAdmin)
                        {
                            //_list = deserialized.AllUsers;
                            //return RedirectToAction("Dashboard", "Home");
                            return View("Dashboard", deserialized);
                        //return RedirectToAction("Dashboard", "Home", deserialized);
                    }
                    return View("Profile", deserialized.AllUsers[0]);
                    }
                    else
                    {
                    ViewBag.Message = "Invalid login details!";
                    return View("Login");
                }
                }
        }
        /// <summary>
        /// The route handling logout
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");
            ViewBag.Message = "Log out successful!";
            return RedirectToAction("Login");
        }
        /// <summary>
        /// Handles the persistence of photo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string UploadedFile(SignupViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        /// <summary>
        /// Loads the signup view
        /// </summary>
        /// <returns></returns>
        [HttpGet("Signup")]
        public IActionResult Signup()
        {
            return View();
        }
        /// <summary>
        /// Signup Post method
        /// </summary>
        /// <returns></returns>
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            string uniqueFileName = UploadedFile(model);

            var user = new RegisterUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Photo = uniqueFileName,
                Password = model.Password,
                Gender = model.Gender
            };

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), 
                    Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "/user/Register";
                    
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //TempData["Profile"] = JsonConvert.SerializeObject(user);
                        return RedirectToAction("Login");
                    }
                    else if (Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("Username", "Username Already Exist");
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
