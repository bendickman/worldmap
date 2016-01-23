using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace TheWorld.Models
{
    public class WorldUser : IdentityUser
    {
        // extend the properties that are provided by IdentityUser i.e. username, password, email
        public DateTime FirstTrip { get; set; }
    }
}