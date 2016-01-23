using System.Collections.Generic;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetAllTripsWithStops();
        void AddTrip(Trip newTrip);
        bool SaveAll();
        Trip GetTripByName(string decodeName);
        void AddStop(string tripName, Stop newStop, string name);
        IEnumerable<Trip> GetUserTripsWithStops(string name);
        Trip GetTripByName(string decodeName, string name);
    }
}