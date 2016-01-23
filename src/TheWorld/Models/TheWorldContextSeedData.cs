using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class TheWorldContextSeedData
    {
        private WorldContext _context;
        private UserManager<WorldUser> _userManager;

        public TheWorldContextSeedData(WorldContext context, UserManager<WorldUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task EnsureSeedDataAsync()
        {
            //add a test user if necessary 
            if (await _userManager.FindByEmailAsync("ben.dickman@theworld.com") == null)
            {
                //add the user
                var newUser = new WorldUser()
                {
                    UserName = "bendickman",
                    Email = "ben.dickman@theworld.com"
                };

                IdentityResult user = await _userManager.CreateAsync(newUser, "P@ssword1");
            }

            // if there are any trips
            if (!_context.Trips.Any())
            {
                // add new data
                var usTrip = new Trip()
                {
                    Name = "US Trips",
                    Created = DateTime.Now,
                    UserName = "bendickman",
                    Stops = new List<Stop>()
                    {
                        new Stop() { Name = "Home", Longitude = 51.969894, Latitude = -0.420675, Order = 0, Arrival = new DateTime(2014, 09, 01) },
                        new Stop() { Name = "San Franciso", Longitude = 37.801075, Latitude = -122.425037, Order = 1, Arrival = new DateTime(2014, 09, 01) },
                        new Stop() { Name = "Los Angeles", Longitude = 34.049957, Latitude = -118.253841, Order = 2, Arrival = new DateTime(2014, 09, 04) },
                        new Stop() { Name = "Home", Longitude = 51.969894, Latitude = -0.420675, Order = 3, Arrival = new DateTime(2014, 09, 11) }
                    }
                };

                _context.Trips.Add(usTrip);
                _context.Stops.AddRange(usTrip.Stops);

                _context.SaveChanges();
            }
        }
    }
}
