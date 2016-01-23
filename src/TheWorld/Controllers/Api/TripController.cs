using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        private ILogger<TripController> _logger;
        private IWorldRepository _repo;

        public TripController(IWorldRepository repo, ILogger<TripController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("")]//empty string use the route on the class
        public JsonResult Get()
        {
            //map the correct type, in this case the ienumerable
            var trips = _repo.GetUserTripsWithStops(User.Identity.Name);
            var results = Mapper.Map<IEnumerable<TripViewModel>>(trips);

            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel tripVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //map from viewModel to model
                    var newTrip = Mapper.Map<Trip>(tripVm);

                    newTrip.UserName = User.Identity.Name;

                    //save model
                    _logger.LogInformation("attempting to save new trip");
                    _repo.AddTrip(newTrip);

                    if (_repo.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;

                        //return the viewModel back to the user, not the model
                        return Json(Mapper.Map<TripViewModel>(newTrip));
                    }

                    
                }
            }
            catch (Exception exc)
            {
                _logger.LogError("Failed to save new trip", exc);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = exc.Message});
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState });
            
            
        }
    }
}
