using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TheWorld.Services
{
    public class CoordService
    {
        private ILogger _logger;

        public CoordService(ILogger<CoordService> logger)
        {
            _logger = logger;
        }

        public async Task<CoordServiceResult> Lookup(string location)
        {
            try
            {
                var result = new CoordServiceResult()
                {
                    Success = false,
                    Message = "Failed lookup"
                };


                //Lookup coordinates
                var encodedLocation = WebUtility.UrlEncode(location);
                var key = Startup.Configuration["AppSettings:BingKey"];

                var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedLocation}&key={key}";

                //system.net.http namespace for httpclient, this is a better compatibility with asp.net 5
                var client = new HttpClient();

                var json = await client.GetStringAsync(url);

                var results = JObject.Parse(json);
                var resources = results["resourceSets"][0]["resources"];

                if (!resources.HasValues)
                {
                    result.Message = $"No locations found for {location}";
                }
                else
                {
                    var confidence = (string)resources[0]["confidence"];

                    if (confidence != "High")
                    {
                        result.Message = $"Could not find a confident match for {location}";
                    }
                    else
                    {
                        var coords = resources[0]["geocodePoints"][0]["coordinates"];
                        result.Longitude = (double)coords[1];
                        result.Latitude = (double)coords[0];
                        result.Success = true;
                        result.Message = "Success";
                    }
                }


                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError($"Failed to lookup coordinates of location: {location}", exc);
                return null;
            }
        }
    }
}
