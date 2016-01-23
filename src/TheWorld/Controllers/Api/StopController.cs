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
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private CoordService _coordService;
        private ILogger<StopController> _logger;
        private IWorldRepository _repo;

        public StopController(IWorldRepository repo, ILogger<StopController> logger, CoordService coordService)
        {
            _repo = repo;
            _logger = logger;
            _coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var decodeName = WebUtility.UrlDecode(tripName);

                var result = _repo.GetTripByName(decodeName, User.Identity.Name);

                if (result == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<StopViewModel>>(result.Stops.OrderBy(s => s.Order)));
            }
            catch (Exception exc)
            {
                _logger.LogError($"Could not find trip with a name of: {tripName}", exc);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error finding trip");
            }
        }

        [HttpPost("")]
        public async Task<JsonResult> Post(string tripName, [FromBody]StopViewModel stopVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(stopVm);

                    //lookup coordinates
                    var coordResult = await _coordService.Lookup(newStop.Name);

                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(coordResult.Message);
                    }

                    newStop.Longitude = coordResult.Longitude;
                    newStop.Latitude = coordResult.Latitude;

                    var decodeName = WebUtility.UrlDecode(tripName);

                    _repo.AddStop(decodeName, newStop, User.Identity.Name);

                    if (_repo.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }


                }
            }
            catch (Exception exc)
            {
                _logger.LogError("Fail to save new stop", exc);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error saving new stop");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Validation failed for new stop");
        }
    }
}
