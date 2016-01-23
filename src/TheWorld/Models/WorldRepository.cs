using Microsoft.Data.Entity;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop newStop, string name)
        {
            
            var Trip = GetTripByName(tripName, name);

            if (Trip.Stops.Count > 0)
            {
                newStop.Order = Trip.Stops.Max(s => s.Order) + 1;
            }
            else
            {
                newStop.Order = 0;
            }
            

            Trip.Stops.Add(newStop);
            
            _context.Stops.Add(newStop);
        }

        public void AddTrip(Trip newTrip)
        {
            try
            {
                _context.Add(newTrip);
            }
            catch (Exception exc)
            {

            }
            
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return _context.Trips.OrderBy(t => t.Name).ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError("Could not get trips from database", exc);
                return null;
            }
            
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return _context.Trips
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError("Could not get trips with stops from database", exc);
                return null;
            }
        }

        public Trip GetTripByName(string decodeName)
        {
            return _context.Trips.Include(t => t.Stops)
                .Where(t => t.Name == decodeName)
                .FirstOrDefault();
            
        }

        public Trip GetTripByName(string decodeName, string name)
        {
            return _context.Trips.Include(t => t.Stops)
                .Where(t => t.Name == decodeName && t.UserName == name)
                .FirstOrDefault();
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string name)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
