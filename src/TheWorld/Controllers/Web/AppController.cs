using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TheWorld.ViewModels;
using TheWorld.Services;
using TheWorld.Models;
using Microsoft.AspNet.Authorization;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IWorldRepository _repo;

        public AppController(IMailService mailService, IWorldRepository repo)
        {
            _mailService = mailService;
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            //var trips = _repo.GetAllTrips();
            
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel contact)
        {
            

            if (ModelState.IsValid)
            {
                var mail = Startup.Configuration["ApplicationSettings:EmailAddress"];

                if (string.IsNullOrWhiteSpace(mail))
                {
                    ModelState.AddModelError("", "Could not send email, configuration error.");
                }

                if (_mailService.SendMail(mail,
                mail,
                $"Contact Request - Name: { contact.Name } ({ contact.Email })",
                $"Message: { contact.Message })"))
                {
                    ModelState.Clear();

                    ViewBag.message = "Thanks for contacting us!";
                }
            }

            return View();
        }
    }
}
